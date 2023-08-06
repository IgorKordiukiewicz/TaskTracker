using Application.Features.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Web.Server.Extensions;

namespace Web.Server.Controllers;

[ApiController]
[Route("projects")]
[Authorize("OrganizationMember")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto model)
    {
        var result = await _mediator.Send(new CreateProjectCommand(model));
        return result.ToHttpResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects([FromHeader] Guid organizationId)
    {
        var result = await _mediator.Send(new GetProjectsQuery(organizationId));
        return result.ToHttpResult();
    }
}
