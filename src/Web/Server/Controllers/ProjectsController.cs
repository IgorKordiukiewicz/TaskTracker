using Application.Features.Projects;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <remarks>
    /// Returns ID of the created project.
    /// </remarks>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response>
    [HttpPost]
    [Authorize(Policy.OrganizationCreateProjects)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto model)
    {
        var result = await _mediator.Send(new CreateProjectCommand(User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get the organization's projects that user belongs to.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response>
    [HttpGet("organization/{organizationId:guid}/user")]
    [Authorize(Policy.OrganizationMember)]
    [ProducesResponseType(typeof(ProjectsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjects(Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsForOrganizationQuery(organizationId, User.GetUserAuthenticationId()));
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
        var result = await _mediator.Send(new GetProjectNavDataQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new project member.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members")]
    [Authorize(Policy.ProjectAddMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddProjectMember(Guid projectId, [FromBody] AddProjectMemberDto model)
    {
        var result = await _mediator.Send(new AddProjectMemberCommand(projectId, model));
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
        var result = await _mediator.Send(new GetProjectMembersQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Remove a member from a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="memberId"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members/{memberId:guid}/remove")]
    [Authorize(Policy.ProjectRemoveMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, Guid memberId)
    {
        var result = await _mediator.Send(new RemoveProjectMemberCommand(projectId, memberId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update role of a project member.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="memberId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members/{memberId:guid}/update-role")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMemberRoleMember(Guid projectId, Guid memberId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await _mediator.Send(new UpdateProjectMemberRoleCommand(projectId, memberId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get info about a project's organization.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/organization")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(ProjectOrganizationVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectOrganization(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectOrganizationQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of roles for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/roles")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(typeof(RolesVM<ProjectPermissions>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectRoles(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectRolesQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a new project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProjectRole(Guid projectId, [FromBody] CreateRoleDto<ProjectPermissions> model)
    {
        var result = await _mediator.Send(new CreateProjectRoleCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="roleId"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/{roleId:guid}/delete")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProjectRole(Guid projectId, Guid roleId)
    {
        var result = await _mediator.Send(new DeleteProjectRoleCommand(projectId, roleId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update name of a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="roleId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/{roleId:guid}/update-name")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRoleName(Guid projectId, Guid roleId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await _mediator.Send(new UpdateProjectRoleNameCommand(projectId, roleId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update permissions of a project role.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="roleId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/{roleId:guid}/update-permissions")]
    [Authorize(Policy.ProjectManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRolePermissions(Guid projectId, Guid roleId, [FromBody] UpdateRolePermissionsDto<ProjectPermissions> model)
    {
        var result = await _mediator.Send(new UpdateProjectRolePermissionsCommand(projectId, roleId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get settings for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/settings")]
    [Authorize(Policy.ProjectManageProject)]
    [ProducesResponseType(typeof(ProjectSettingsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjectSettings(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectSettingsQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a project's name.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/update-name")]
    [Authorize(Policy.ProjectManageProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProjectName(Guid projectId, [FromBody] UpdateProjectNameDto model)
    {
        var result = await _mediator.Send(new UpdateProjectNameCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/delete")]
    [Authorize(Policy.ProjectManageProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var result = await _mediator.Send(new DeleteProjectCommand(projectId));
        return result.ToHttpResult();
    }
}
