using Application.Features.Projects;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="mediator"></param>
[ApiController]
[Route("projects")]
[Authorize]
public class ProjectsController(IMediator mediator) 
    : ControllerBase
{
    /// <summary>
    /// Create a new project
    /// </summary>
    /// <remarks>
    /// Returns ID of the created project.
    /// </remarks>
    /// <param name="model"></param>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto model)
    {
        var result = await mediator.Send(new CreateProjectCommand(User.GetUserId(), model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get projects the user belongs to.
    /// </summary>
    [HttpGet("")]
    [Authorize]
    [ProducesResponseType(typeof(ProjectsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjects()
    {
        var result = await mediator.Send(new GetProjectsQuery(User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get navigation data for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/nav-data")]
    [ProducesResponseType(typeof(ProjectNavigationVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectNavData(Guid projectId)
    {
        var result = await mediator.Send(new GetProjectNavDataQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get members of a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/members")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(ProjectMembersVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectMembers(Guid projectId)
    {
        var result = await mediator.Send(new GetProjectMembersQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a invitation to project for a given user.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project or user not found.</response> 
    [HttpPost("{projectId:guid}/invitations")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProjectInvitation(Guid projectId, [FromBody] CreateProjectInvitationDto model)
    {
        var result = await mediator.Send(new CreateProjectInvitationCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get all invitations of a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response> 
    [HttpGet("{projectId:guid}/invitations")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(typeof(ProjectInvitationsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectInvitationsForProject(Guid projectId)
    {
        var result = await mediator.Send(new GetProjectInvitationsQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get pending invitations for the user.
    /// </summary>
    /// <response code="404">User not found.</response> 
    [HttpGet("invitations")]
    [ProducesResponseType(typeof(UserProjectInvitationsVM), 200)]
    public async Task<IActionResult> GetProjectInvitationsForUser()
    {
        var result = await mediator.Send(new GetProjectInvitationsForUserQuery(User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Decline project invitation (by user).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Project with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/decline")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeclineProjectInvitation(Guid invitationId)
    {
        var result = await mediator.Send(new DeclineProjectInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Accept project invitation (by user).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Project with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/accept")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AcceptProjectInvitation(Guid invitationId)
    {
        var result = await mediator.Send(new AcceptProjectInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Cancel project invitation (by project member).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Project with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/cancel")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelProjectInvitation(Guid invitationId)
    {
        var result = await mediator.Send(new CancelProjectInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Remove a member from a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members/remove")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, [FromBody] RemoveProjectMemberDto model)
    {
        var result = await mediator.Send(new RemoveProjectMemberCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update role of a project member.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members/role")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMemberRoleMember(Guid projectId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await mediator.Send(new UpdateProjectMemberRoleCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of roles for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/roles")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(RolesVM<ProjectPermissions>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectRoles(Guid projectId)
    {
        var result = await mediator.Send(new GetProjectRolesQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a new project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles")]
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProjectRole(Guid projectId, [FromBody] CreateRoleDto<ProjectPermissions> model)
    {
        var result = await mediator.Send(new CreateProjectRoleCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/delete")]
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProjectRole(Guid projectId, [FromBody] DeleteRoleDto model)
    {
        var result = await mediator.Send(new DeleteProjectRoleCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update name of a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/name")]
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRoleName(Guid projectId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await mediator.Send(new UpdateProjectRoleNameCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update permissions of a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/permissions")]
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRolePermissions(Guid projectId, [FromBody] UpdateRolePermissionsDto<ProjectPermissions> model)
    {
        var result = await mediator.Send(new UpdateProjectRolePermissionsCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get settings for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/settings")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(typeof(ProjectSettingsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectSettings(Guid projectId)
    {
        var result = await mediator.Send(new GetProjectSettingsQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a project's name.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/name")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProjectName(Guid projectId, [FromBody] UpdateProjectNameDto model)
    {
        var result = await mediator.Send(new UpdateProjectNameCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/delete")]
    [Authorize(Policy.ProjectOwner)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var result = await mediator.Send(new DeleteProjectCommand(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get user's permission in a given project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/permissions")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(UserProjectPermissionsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserProjectPermissions(Guid projectId)
    {
        var result = await mediator.Send(new GetUserProjectPermissionsQuery(User.GetUserId(), projectId));
        return result.ToHttpResult();
    }
}
