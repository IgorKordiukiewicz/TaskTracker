using Application.Features.Workflows;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Web.Server.Extensions;

namespace Web.Server.Controllers;

[ApiController]
[Route("workflows")]
[Authorize]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("project/{projectId:guid}")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetWorkflow(Guid projectId)
    {
        var result = await _mediator.Send(new GetWorkflowForProjectQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{workflowId:guid}/statuses")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> AddStatus(Guid workflowId, AddWorkflowStatusDto model)
    {
        var result = await _mediator.Send(new AddWorkflowTaskStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }
}
