using Application.Features.Workflows;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="mediator"></param>
[ApiController]
[Route("workflows")]
[Authorize]
[ProducesResponseType(200)]
[ProducesResponseType(400)]
[ProducesResponseType(500)]
[Produces("application/json")]
public class WorkflowsController(IMediator mediator) 
    : ControllerBase
{
    /// <summary>
    /// Get project's workflow.
    /// </summary>
    /// <param name="projectId"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpGet]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(typeof(WorkflowVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWorkflow([FromQuery] Guid projectId)
    {
        var result = await mediator.Send(new GetWorkflowForProjectQuery(projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new workflow status.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/statuses")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddStatus(Guid workflowId, AddWorkflowStatusDto model)
    {
        var result = await mediator.Send(new AddWorkflowTaskStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a workflow status.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/statuses/delete")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteStatus(Guid workflowId, DeleteWorkflowStatusDto model)
    {
        var result = await mediator.Send(new DeleteWorkflowStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new workflow transition.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/transitions")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddTransition(Guid workflowId, AddWorkflowTransitionDto model)
    {
        var result = await mediator.Send(new AddWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a workflow transition.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/transitions/delete")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTransition(Guid workflowId, DeleteWorkflowTransitionDto model)
    {
        var result = await mediator.Send(new DeleteWorkflowTransitionCommand(workflowId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Change a workflow's initial status.
    /// </summary>
    /// <param name="workflowId"></param>
    /// <param name="model"></param>
    /// <response code="404">Workflow not found.</response>
    [HttpPost("{workflowId:guid}/initial-status")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ChangeInitialStatus(Guid workflowId, ChangeInitialWorkflowStatusDto model)
    {
        var result = await mediator.Send(new ChangeInitialWorkflowStatusCommand(workflowId, model));
        return result.ToHttpResult();
    }
}
