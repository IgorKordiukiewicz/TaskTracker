using Application.Features.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize("OrganizationMember")]
    public async Task<IActionResult> CreateProject([FromRoute] Guid organizationId, [FromBody] CreateProjectDto model)
    {
        var result = await _mediator.Send(new CreateProjectCommand(organizationId, User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }

    [HttpGet("organization/{organizationId:guid}/user")]
    [Authorize("OrganizationMember")]
    public async Task<IActionResult> GetProjects(Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsForOrganizationQuery(organizationId, User.GetUserAuthenticationId()));
        return result.ToHttpResult();
    }

    [HttpPost("{projectId:guid}/members")]
    [Authorize("ProjectMember")]
    public async Task<IActionResult> AddProjectMember([FromRoute] Guid projectId, [FromBody] AddProjectMemberDto model)
    {
        var result = await _mediator.Send(new AddProjectMemberCommand(projectId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{projectId:guid}/members")]
    [Authorize("ProjectMember")]
    public async Task<IActionResult> GetProjectMembers([FromRoute] Guid projectId) // TODO: Unify the usage of From.. attributes across the api
    {
        var result = await _mediator.Send(new GetProjectMembersQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{projectId:guid}/members/{memberId:guid}/remove")] // TODO: Use HttpDelete?
    [Authorize("ProjectMember")]
    public async Task<IActionResult> RemoveProjectMember([FromRoute] Guid projectId, [FromRoute] Guid memberId)
    {
        var result = await _mediator.Send(new RemoveProjectMemberCommand(projectId, memberId));
        return result.ToHttpResult();
    }
}
