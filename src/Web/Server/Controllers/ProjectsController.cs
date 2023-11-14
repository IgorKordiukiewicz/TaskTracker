using Application.Features.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Dtos;
using Web.Server.Extensions;

namespace Web.Server.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("organization/{organizationId:guid}")]
    [Authorize(Policy.OrganizationProjectsEditor)]
    public async Task<IActionResult> CreateProject(Guid organizationId, [FromBody] CreateProjectDto model)
    {
        var result = await _mediator.Send(new CreateProjectCommand(organizationId, User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }

    [HttpGet("organization/{organizationId:guid}/user")]
    [Authorize(Policy.OrganizationMember)]
    public async Task<IActionResult> GetProjects(Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsForOrganizationQuery(organizationId, User.GetUserAuthenticationId())); // TODO: rename to GetProjectsForUser ?, and api: projects/?organizationId
        return result.ToHttpResult();
    }

    [HttpGet("{projectId:guid}/nav-data")]
    public async Task<IActionResult> GetProjectNavData(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectNavDataQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{projectId:guid}/members")]
    [Authorize(Policy.ProjectMembersEditor)]
    public async Task<IActionResult> AddProjectMember(Guid projectId, [FromBody] AddProjectMemberDto model)
    {
        var result = await _mediator.Send(new AddProjectMemberCommand(projectId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{projectId:guid}/members")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetProjectMembers(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectMembersQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{projectId:guid}/members/{memberId:guid}/remove")] // TODO: Use HttpDelete?
    [Authorize(Policy.ProjectMembersEditor)]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, Guid memberId)
    {
        var result = await _mediator.Send(new RemoveProjectMemberCommand(projectId, memberId));
        return result.ToHttpResult();
    }

    [HttpGet("{projectId:guid}/organization")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetProjectOrganization(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectOrganizationQuery(projectId));
        return result.ToHttpResult();
    }
}
