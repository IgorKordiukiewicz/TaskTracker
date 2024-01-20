using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Shared.Authorization;
using Shared.Enums;
using Shared.ViewModels;
using Web.Client.Common;

namespace Web.Client.RequirementHandlers;

public class ProjectMemberRequirementHandler : MemberRequirementHandler<ProjectMemberRequirement, ProjectPermissions>
{
    public ProjectMemberRequirementHandler(UserDataService userDataService, NavigationManager navigationManager)
        : base(userDataService, navigationManager, "project", x => x.PermissionsByProject)
    {
    }
}

public class OrganizationMemberRequirementHandler : MemberRequirementHandler<OrganizationMemberRequirement, OrganizationPermissions>
{
    public OrganizationMemberRequirementHandler(UserDataService userDataService, NavigationManager navigationManager)
        : base(userDataService, navigationManager, "org", x => x.PermissionsByOrganization)
    {
    }
}

public abstract class MemberRequirementHandler<TAuthorizationRequirement, TPermissions> : AuthorizationHandler<MemberRequirement<TPermissions>>
    where TAuthorizationRequirement : IAuthorizationRequirement
    where TPermissions : struct, Enum
{
    private readonly UserDataService _userDataService;
    private readonly NavigationManager _navigationManager;
    private readonly string _routeKey;
    private readonly Func<UserVM, IReadOnlyDictionary<Guid, TPermissions>> _permissionsSelector;

    public MemberRequirementHandler(UserDataService userDataService, NavigationManager navigationManager, 
        string routeKey, Func<UserVM, IReadOnlyDictionary<Guid, TPermissions>> permissionsSelector)
    {
        _userDataService = userDataService;
        _navigationManager = navigationManager;
        _routeKey = routeKey;
        _permissionsSelector = permissionsSelector;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MemberRequirement<TPermissions> requirement)
    {
        var currentUser = _userDataService.CurrentUserVM;
        if (currentUser is null)
        {
            context.Fail();
            return;
        }

        var url = _navigationManager.Uri;
        var route = $"/{_routeKey}/";
        var routeStartIndex = url.IndexOf(route);
        var entityIdStr = url.Substring(routeStartIndex + route.Length, 36); // Guid is 36 characters long
        if (!Guid.TryParse(entityIdStr, out var entityId))
        {
            context.Fail();
            return;
        }

        if (!_permissionsSelector(currentUser).TryGetValue(entityId, out var permissions))
        {
            context.Fail();
            return;
        }

        if (requirement.Permissions is not null && !permissions.HasFlag(requirement.Permissions.Value))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;
    }
}