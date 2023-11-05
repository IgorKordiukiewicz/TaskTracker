using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.ViewModels;
using Web.Client.Components;

namespace Web.Client.Common;

public class UserDataService
{
    private readonly RequestHandler _requestHandler;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserDataService(RequestHandler requestHandler, AuthenticationStateProvider authenticationStateProvider)
    {
        _requestHandler = requestHandler;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public UserVM? CurrentUserVM { get; private set; }

    public event Action? SignedIn;
    public event Action? SignedOut;

    public async Task OnSignIn()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = state.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
        {
            var userAuthId = state.User.Claims.First(x => x.Type == "sub").Value;
            CurrentUserVM = await _requestHandler.GetAsync<UserVM>($"users/{userAuthId}/data");
            
            if(SignedIn is not null)
            {
                SignedIn();
            }
        }
    }

    public void OnSignOut()
    {
        CurrentUserVM = null;

        if(SignedOut is not null)
        {
            SignedOut();
        }
    }
}
