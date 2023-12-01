using Application.Features.Workflows;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
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
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> GetWorkflow(Guid projectId)
    {
        var result = await _mediator.Send(new GetWorkflowForProjectQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{workflowId:guid}/statuses")]
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> AddStatus(Guid workflowId, AddWorkflowStatusDto model)
    {
        var result = await _mediator.Send(new AddWorkflowTaskStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{workflowId:guid}/statuses/{statusId:guid}/can-be-deleted")]
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> CanStatusBeDeleted(Guid workflowId, Guid statusId)
    {
        var result = await _mediator.Send(new CanWorkflowStatusBeDeletedQuery(workflowId, statusId));
        return result.ToHttpResult();
    }

    [HttpPost("{workflowId:guid}/statuses/{statusId:guid}/delete")]
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> DeleteStatus(Guid workflowId, Guid statusId)
    {
        var result = await _mediator.Send(new DeleteWorkflowStatusCommand(workflowId, statusId));
        return result.ToHttpResult();
    }

    [HttpPost("{workflowId:guid}/transitions")]
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> AddTransition(Guid workflowId, AddWorkflowTransitionDto model)
    {
        var result = await _mediator.Send(new AddWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{workflowId:guid}/transitions/delete")]
    [Authorize(Policy.ProjectManageWorkflows)]
    public async Task<IActionResult> DeleteTransition(Guid workflowId, DeleteWorkflowTransitionDto model)
    {
        var result = await _mediator.Send(new DeleteWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }
}
