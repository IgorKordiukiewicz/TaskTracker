using Flurl;
using MudBlazor;
using Shared.Dtos;
using Shared.ViewModels;

namespace Web.Client.Services;

public class UsersService : ApiService
{
    public UsersService(HttpClient httpClient, ISnackbar snackbar) : base(httpClient, snackbar)
    {
    }

    public async Task<bool> IsRegistered(string userAuthenticationId)
        => await Get<bool>($"users/{userAuthenticationId}/is-registered");

    public async Task<UserVM?> Get(string userAuthenticationId)
        => await Get<UserVM>($"users/{userAuthenticationId}/data");

    public async Task<bool> Register(UserRegistrationDto model)
        => await Post("users/register", model);

    public async Task<UsersSearchVM?> GetAvailableForInvitation(Guid organizationId, string searchValue)
    {
        var url = $"users/available-for-invitation"
            .SetQueryParam("searchValue", searchValue)
            .SetQueryParam("organizationId", organizationId)
            .ToString();
        return await Get<UsersSearchVM>(url);
    }

    public async Task<UsersSearchVM?> GetAvailableForProject(Guid projectId, Guid organizationId)
    {
        var url = "users/available-for-project"
                .SetQueryParam("projectId", projectId)
                .SetQueryParam("organizationId", organizationId)
                .ToString();
        return await Get<UsersSearchVM>(url);
    }

    public async Task<bool> UpdateName(Guid userId, UpdateUserNameDto model)
        => await Post($"users/{userId}/update-name", model);

    public async Task<UsersPresentationDataVM?> GetAllPresentationData()
        => await Get<UsersPresentationDataVM>("users/presentation");
}
