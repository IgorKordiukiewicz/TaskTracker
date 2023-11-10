﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Web.Client.Common;

namespace Web.Client.Requirements;

public class ProjectMemberRequirement : IAuthorizationRequirement
{
}

public class ProjectMemberRequirementHandler : AuthorizationHandler<ProjectMemberRequirement>
{
    private readonly UserDataService _userDataService;
    private readonly NavigationManager _navigationManager;

    public ProjectMemberRequirementHandler(UserDataService userDataService, NavigationManager navigationManager)
    {
        _userDataService = userDataService;
        _navigationManager = navigationManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectMemberRequirement requirement)
    {
        var currentUser = _userDataService.CurrentUserVM;
        if (currentUser is null)
        {
            context.Fail();
            return;
        }

        // TODO: Find a better method of acquiring current projectId
        var url = _navigationManager.Uri;
        var projectRouteStartIndex = url.IndexOf("/project/");
        var projectIdStr = url.Substring(projectRouteStartIndex + 9, 36); // Guid is 36 characters long
        if (!Guid.TryParse(projectIdStr, out var projectId))
        {
            context.Fail();
            return;
        }

        if (!currentUser.ProjectsMember.Contains(projectId))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;
    }
}