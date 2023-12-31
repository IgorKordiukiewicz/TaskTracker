﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Shared.Authorization;
using Web.Client.Common;

namespace Web.Client.RequirementHandlers;

// TODO: Extract a common base class for organization & project handlers
public class OrganizationMemberRequirementHandler : AuthorizationHandler<OrganizationMemberRequirement>
{
    private readonly UserDataService _userDataService;
    private readonly NavigationManager _navigationManager;

    public OrganizationMemberRequirementHandler(UserDataService userDataService, NavigationManager navigationManager)
    {
        _userDataService = userDataService;
        _navigationManager = navigationManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationMemberRequirement requirement)
    {
        var currentUser = _userDataService.CurrentUserVM;
        if(currentUser is null)
        {
            context.Fail();
            return;
        }

        // TODO: Find a better method of acquiring current organizationId
        var url = _navigationManager.Uri;
        var orgRouteStartIndex = url.IndexOf("/org/");
        var orgIdStr = url.Substring(orgRouteStartIndex + 5, 36); // Guid is 36 characters long
        if(!Guid.TryParse(orgIdStr, out var orgId))
        {
            context.Fail();
            return;
        }

        if(!currentUser.PermissionsByOrganization.TryGetValue(orgId, out var permissions))
        {
            context.Fail();
            return;
        }
        
        if(requirement.Permissions is not null && !permissions.HasFlag(requirement.Permissions.Value))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;
    }
}