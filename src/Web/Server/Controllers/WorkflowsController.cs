using Application.Features.Workflows;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("workflows")]
[Authorize]
[ProducesResponseType(200)]
[ProducesResponseType(400)]
[ProducesResponseType(500)]
[Produces("application/json")]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public WorkflowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get project's workflow.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpGet]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(typeof(WorkflowVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWorkflow([FromQuery] Guid projectId)
    {
        var result = await _mediator.Send(new GetWorkflowForProjectQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new workflow status.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/statuses")]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddStatus(Guid workflowId, AddWorkflowStatusDto model)
    {
        var result = await _mediator.Send(new AddWorkflowTaskStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Check whether status can be deleted.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="statusId"></param>
    /// <response code="404">Workflow or status not found.</response>
    [HttpGet("{workflowId:guid}/statuses/{statusId:guid}/can-be-deleted")]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CanStatusBeDeleted(Guid workflowId, Guid statusId)
    {
        var result = await _mediator.Send(new CanWorkflowStatusBeDeletedQuery(workflowId, statusId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a workflow status.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="statusId"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/statuses/{statusId:guid}/delete")]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteStatus(Guid workflowId, Guid statusId)
    {
        var result = await _mediator.Send(new DeleteWorkflowStatusCommand(workflowId, statusId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new workflow transition.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/transitions")]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddTransition(Guid workflowId, AddWorkflowTransitionDto model)
    {
        var result = await _mediator.Send(new AddWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a workflow transition.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/transitions/delete")]
    [Authorize(Policy.ProjectManageWorkflows)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTransition(Guid workflowId, DeleteWorkflowTransitionDto model)
    {
        var result = await _mediator.Send(new DeleteWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }
}
