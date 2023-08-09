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

    [HttpGet("organization/{organizationId:guid}")]
    [Authorize("OrganizationMember")]
    public async Task<IActionResult> GetProjects(Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsQuery(organizationId)); // TODO: Rename file from Get to GetForOrg or sth?
        return result.ToHttpResult();
    }

    [HttpPost("{projectId:guid}/members")]
    [Authorize("ProjectMember")]
    public async Task<IActionResult> AddProjectMember([FromRoute] Guid projectId, [FromBody] AddProjectMemberDto model)
    {
        var result = await _mediator.Send(new AddProjectMemberCommand(projectId, model));
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
