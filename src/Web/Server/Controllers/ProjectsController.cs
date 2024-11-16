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
    [Authorize(Policy.OrganizationEditProjects)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto model)
    {
        var result = await _mediator.Send(new CreateProjectCommand(User.GetUserId(), model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get the organization's projects that user belongs to.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response>
    [HttpGet("")]
    [Authorize(Policy.OrganizationMember)]
    [ProducesResponseType(typeof(ProjectsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProjects([FromQuery] Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsForOrganizationQuery(organizationId, User.GetUserId()));
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
    [Authorize(Policy.ProjectEditTasks)]
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
    [HttpPost("{projectId:guid}/members/remove")]
    [Authorize(Policy.ProjectEditMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, [FromBody] RemoveProjectMemberDto model)
    {
        var result = await _mediator.Send(new RemoveProjectMemberCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update role of a project member.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="memberId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/members/role")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMemberRoleMember(Guid projectId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await _mediator.Send(new UpdateProjectMemberRoleCommand(projectId, model));
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
    [Authorize(Policy.ProjectEditRoles)]
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
    [Authorize(Policy.ProjectEditRoles)]
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
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response>
    [HttpPost("{projectId:guid}/roles/delete")]
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProjectRole(Guid projectId, [FromBody] DeleteRoleDto model)
    {
        var result = await _mediator.Send(new DeleteProjectRoleCommand(projectId, model));
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
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRoleName(Guid projectId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await _mediator.Send(new UpdateProjectRoleNameCommand(projectId, model));
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
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRolePermissions(Guid projectId, [FromBody] UpdateRolePermissionsDto<ProjectPermissions> model)
    {
        var result = await _mediator.Send(new UpdateProjectRolePermissionsCommand(projectId, model));
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
        var result = await _mediator.Send(new GetProjectSettingsQuery(projectId));
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
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var result = await _mediator.Send(new DeleteProjectCommand(projectId));
        return result.ToHttpResult();
    }
}
