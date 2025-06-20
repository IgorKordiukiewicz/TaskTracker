using Application.Features.Tasks;
using OneOf;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="mediator"></param>
[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController(IMediator mediator) 
    : ControllerBase
{
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
    [Authorize(Policy.ProjectEditRoles)]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateTask([FromHeader] Guid projectId, [FromBody] CreateTaskDto model)
    {
        var result = await mediator.Send(new CreateTaskCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of tasks for a project.
    /// </summary>
    /// <remarks>
    /// Use one of 
    /// </remarks>
    /// <param name="projectId"></param>
    /// <param name="ids">List of tasks IDs to return. Returns all tasks if empty.</param>
    /// <response code="404">Project's workflow not found.</response> 
    [HttpGet]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TasksVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTasks([FromHeader] Guid projectId, [FromQuery] IEnumerable<Guid> ids)
    {
        var result = await mediator.Send(new GetTasksQuery(projectId, OneOf<int, IEnumerable<Guid>>.FromT1(ids)));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a single task for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="shortId"></param>
    /// <response code="404">Project's workflow not found.</response> 
    [HttpGet("{shortId:int}")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TasksVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTask([FromHeader] Guid projectId, int shortId)
    {
        var result = await mediator.Send(new GetTasksQuery(projectId, shortId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's status.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task or task status not found.</response> 
    [HttpPost("{taskId:guid}/status")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, UpdateTaskStatusDto model)
    {
        var result = await mediator.Send(new UpdateTaskStatusCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's priority.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/priority")]
    [Authorize(Policy.ProjectEditProject)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskPriority(Guid taskId, UpdateTaskPriorityDto model)
    {
        var result = await mediator.Send(new UpdateTaskPriorityCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's assignee.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task or project member not found.</response> 
    [HttpPost("{taskId:guid}/assignee")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskAssignee(Guid taskId, UpdateTaskAssigneeDto model)
    {
        var result = await mediator.Send(new UpdateTaskAssigneeCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's title.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/title")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskTitle(Guid taskId, UpdateTaskTitleDto model)
    {
        var result = await mediator.Send(new UpdateTaskTitleCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's description.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/description")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskDescription(Guid taskId, UpdateTaskDescriptionDto model)
    {
        var result = await mediator.Send(new UpdateTaskDescriptionCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete a task.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/delete")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var result = await mediator.Send(new DeleteTaskCommand(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new task comment.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response> 
    [HttpPost("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddTaskComment(Guid taskId, AddTaskCommentDto model)
    {
        var result = await mediator.Send(new AddTaskCommentCommand(taskId, User.GetUserId(), model));
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
        var result = await mediator.Send(new GetTaskCommentsQuery(taskId));
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
        var result = await mediator.Send(new GetTaskActivitiesQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new entry to task's logged time.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response>
    [HttpPost("{taskId:guid}/logged-time")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> LogTaskTime(Guid taskId, LogTaskTimeDto model)
    {
        var result = await mediator.Send(new LogTaskTimeCommand(User.GetUserId(), taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a task's estimated time.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response>
    [HttpPost("{taskId:guid}/estimated-time")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskEstimatedTime(Guid taskId, UpdateTaskEstimatedTimeDto model)
    {
        var result = await mediator.Send(new UpdateTaskEstimatedTimeCommand(taskId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new hierarchical task relationship for given tasks.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task relationship manager not found.</response>
    [HttpPost("relationships/hierarchical")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddHierarchicalTaskRelationship([FromHeader] Guid projectId, AddHierarchicalTaskRelationshipDto model)
    {
        var result = await mediator.Send(new AddHierarchicalTaskRelationshipCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Remove the given task hierarchical relationship.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="model"></param>
    /// <response code="404">Task relationship manager not found.</response>
    [HttpPost("relationships/hierarchical/remove")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveHierarchicalTaskRelationship([FromHeader] Guid projectId, RemoveHierarchicalTaskRelationshipDto model)
    {
        var result = await mediator.Send(new RemoveHierarchicalTaskRelationshipCommand(projectId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a task's relationships.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="404">Task not found.</response>
    [HttpGet("{taskId:guid}/relationships")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TaskRelationshipsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskRelationships(Guid taskId)
    {
        var result = await mediator.Send(new GetTaskRelationshipsQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get tasks that can be assigned as the given task's children.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="404">Task not found.</response>
    [HttpGet("{taskId:guid}/available-children")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(typeof(TaskAvailableChildrenVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskAvailableChildren(Guid taskId)
    {
        var result = await mediator.Send(new GetTaskAvailableChildrenQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update the layout of the board.
    /// </summary>
    /// <param name="model"></param>
    /// <response code="404">Task not found.</response>
    [HttpPost("update-board")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskBoard([FromBody] UpdateTaskBoardDto model)
    {
        var result = await mediator.Send(new UpdateTaskBoardCommand(model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Add a new attachment to the given task.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="file"></param>
    /// <response code="200">Task attachment added.</response>
    /// <response code="404">Task not found.</response>
    [HttpPost("{taskId:guid}/attachments")]
    [Authorize(Policy.ProjectEditTasks)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddAttachment(Guid taskId, [FromForm] IFormFile file)
    {
        var result = await mediator.Send(new AddTaskAttachmentCommand(taskId, file));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of attachments for the given task.
    /// </summary>
    /// <param name="taskId"></param>
    /// <response code="200">List of task attachments.</response>
    /// <response code="404">Task not found.</response>
    [HttpGet("{taskId:guid}/attachments")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TaskAttachmentsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskAttachments(Guid taskId)
    {
        var result = await mediator.Send(new GetTaskAttachmentsQuery(taskId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a download url for the given attachment.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="attachmentName"></param>
    /// <response code="200">Download url for the given attachment.</response>
    /// <response code="400">Failure getting the download url.</response>
    /// <response code="404">Task not found.</response>
    [HttpGet("{taskId:guid}/attachments/download")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTaskAttachmentDownloadUrl(Guid taskId, [FromQuery] string attachmentName)
    {
        var result = await mediator.Send(new DownloadTaskAttachmentQuery(taskId, attachmentName));
        return result.ToHttpResult();
    }
}
