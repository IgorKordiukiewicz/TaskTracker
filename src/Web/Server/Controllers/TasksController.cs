using Application.Features.Tasks;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <remarks>
    /// Returns ID of the created task.
    /// </remarks>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Project not found.</response> 
    [HttpPost]
    [Authorize(Policy.ProjectCreateTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateTask([FromHeader] Guid projectId, [FromBody] CreateTaskDto model)
    {
        var result = await _mediator.Send(new CreateTaskCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of tasks for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="shortIds">List of tasks IDs to return. Returns all tasks if empty.</param>
    /// <response code="404">Project's workflow not found.</response> 
    [HttpGet]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TasksVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTasks([FromHeader] Guid projectId, [FromQuery] IEnumerable<int> shortIds)
    {
        var result = await _mediator.Send(new GetTasksQuery(projectId, shortIds));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's status.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task or task status not found.</response> 
    [HttpPost("{taskId:guid}/update-status")]
    [Authorize(Policy.ProjectTransitionTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, UpdateTaskStatusDto model)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's priority.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/update-priority")]
    [Authorize(Policy.ProjectModifyTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskPriority(Guid taskId, UpdateTaskPriorityDto model)
    {
        var result = await _mediator.Send(new UpdateTaskPriorityCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's assignee.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task or project member not found.</response> 
    [HttpPost("{taskId:guid}/update-assignee")]
    [Authorize(Policy.ProjectAssignTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskAssignee(Guid taskId, UpdateTaskAssigneeDto model)
    {
        var result = await _mediator.Send(new UpdateTaskAssigneeCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's description.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/update-description")]
    [Authorize(Policy.ProjectAssignTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskDescription(Guid taskId, UpdateTaskDescriptionDto model)
    {
        var result = await _mediator.Send(new UpdateTaskDescriptionCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new task comment.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectAddComments)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddTaskComment(Guid taskId, AddTaskCommentDto model)
    {
        var result = await _mediator.Send(new AddTaskCommentCommand(taskId, User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of a task's comments.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="404">Task not found.</response> 
    [HttpGet("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TaskCommentsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskComments(Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskCommentsQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of task's activities.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="404">Task not found.</response> 
    [HttpGet("{taskId:guid}/activities")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TaskActivitiesVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskActivities(Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskActivitiesQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new entry to task's logged time.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response>
    [HttpPost("{taskId:guid}/log_time")]
    [Authorize(Policy.ProjectLogTimeOnTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> LogTaskTime(Guid taskId, LogTaskTimeDto model)
    {
        var result = await _mediator.Send(new LogTaskTimeCommand(User.GetUserAuthenticationId(), taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's estimated time.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response>
    [HttpPost("{taskId:guid}/update-estimated-time")]
    [Authorize(Policy.ProjectEstimateTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskEstimatedTime(Guid taskId, UpdateTaskEstimatedTimeDto model)
    {
        var result = await _mediator.Send(new UpdateTaskEstimatedTimeCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new hierarchical task relationship for given tasks.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task relationship manager not found.</response>
    [HttpPost("relationships/hierarchical")]
    [Authorize(Policy.ProjectModifyTasks)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddHierarchicalTaskRelationship([FromHeader] Guid projectId, AddHierarchicalTaskRelationshipDto model)
    {
        var result = await _mediator.Send(new AddHierarchicalTaskRelationshipCommand(projectId, model));
        return result.ToHttpResult();
    }
}
