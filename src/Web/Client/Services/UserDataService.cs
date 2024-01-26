using Microsoft.AspNetCore.Components.Authorization;
using Shared.Enums;
using Shared.ViewModels;

namespace Web.Client.Services;

public class UserDataService
{
    private readonly UsersService _usersService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserDataService(UsersService usersService, AuthenticationStateProvider authenticationStateProvider)
    {
        _usersService = usersService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public UserVM? CurrentUserVM { get; private set; }

    public event Action? SignedIn;
    public event Action? SignedOut;

    public async Task UpdateUserData()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = state.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
        {
            var userAuthId = state.User.Claims.First(x => x.Type == "sub").Value;
            CurrentUserVM = await _usersService.Get(userAuthId);

            if (SignedIn is not null)
            {
                SignedIn();
            }
        }
    }

    public void OnSignOut()
    {
        CurrentUserVM = null;

        if (SignedOut is not null)
        {
            SignedOut();
        }
    }

    public bool HasOrganizationPermissions(Guid orgId, OrganizationPermissions permissions)
        => HasPermission(orgId, permissions, x => x.PermissionsByOrganization);

    public bool HasProjectPermissions(Guid projectId, ProjectPermissions permissions)
        => HasPermission(projectId, permissions, x => x.PermissionsByProject);

    private bool HasPermission<TPermissions>(Guid entityId, TPermissions permissions, Func<UserVM, IReadOnlyDictionary<Guid, TPermissions>> permissionsByEntitySelector)
        where TPermissions : struct, Enum
    {
        if (CurrentUserVM is null)
        {
            return false;
        }

        if (!permissionsByEntitySelector(CurrentUserVM).TryGetValue(entityId, out var userPermissions))
        {
            return false;
        }

        return userPermissions.HasFlag(permissions);
    }
}
