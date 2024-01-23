using MudBlazor;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
namespace Web.Client.Services;

public record Headers(IEnumerable<(string Name, string Value)> Values)
{
    public static Headers From(params (string Name, string Value)[] values)
    {
        return new(values);
    }
}

public abstract class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISnackbar _snackbar;

    public ApiService(HttpClient httpClient, ISnackbar snackbar)
    {
        _httpClient = httpClient;
        _snackbar = snackbar;
        _snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
    }

    protected async Task<TResponse?> Get<TResponse>(string url, Headers? headers = null)
        where TResponse : class
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Get, url, headers);
        return await GetResponseContent<TResponse>(await _httpClient.SendAsync(requestMessage));
    }

    protected async Task<bool> Post<TBody>(string url, TBody? body, Headers? headers = null)
        where TBody : class
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, url, headers);
        if(body is not null)
        {
            SetRequestMessageContent(requestMessage, body);
        }
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    protected async Task<bool> Post(string url, Headers? headers = null)
        => await Post(url, (object?)null, headers);

    private HttpRequestMessage CreateRequestMessage(HttpMethod method, string url, Headers? headers = null)
    {
        var message = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri(_httpClient.BaseAddress + url)
        };

        if (headers is not null)
        {
            foreach (var (name, value) in headers.Values)
            {
                message.Headers.Add(name, value);
            }
        }

        return message;
    }

    private async Task<TResponse?> GetResponseContent<TResponse>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        else
        {
            await DisplayErrorMessageBox(response);
            return default;
        }
    }

    private async Task<bool> GetPostResponseResult(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            await DisplayErrorMessageBox(response);
            return false;
        }
    }

    private static void SetRequestMessageContent<TBody>(HttpRequestMessage message, TBody? body)
        where TBody : class
    {
        if (body is null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        message.Content = content;
    }

    private async Task DisplayErrorMessageBox(HttpResponseMessage response)
    {
        var message = response.StatusCode != HttpStatusCode.InternalServerError ? await response.Content.ReadAsStringAsync() : string.Empty;
        _snackbar.Add($"{(int)response.StatusCode} {response.ReasonPhrase}: \n {message}", Severity.Error);
    }
}
