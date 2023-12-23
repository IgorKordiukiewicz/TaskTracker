using Application.Features.Tasks;

namespace Web.Server.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy.ProjectCreateTasks)]
    public async Task<IActionResult> CreateTask([FromHeader] Guid projectId, [FromBody] CreateTaskDto model)
    {
        var result = await _mediator.Send(new CreateTaskCommand(projectId, model));
        return result.ToHttpResult();
    }

    [HttpGet]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetTasks([FromHeader] Guid projectId)
    {
        var result = await _mediator.Send(new GetAllTasksQuery(projectId));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/update-status/{newStatusId:guid}")]
    [Authorize(Policy.ProjectTransitionTasks)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, Guid newStatusId)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(taskId, newStatusId));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/update-priority")]
    [Authorize(Policy.ProjectModifyTasks)]
    public async Task<IActionResult> UpdateTaskPriority(Guid taskId, UpdateTaskPriorityDto model)
    {
        var result = await _mediator.Send(new UpdateTaskPriorityCommand(taskId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/update-assignee")]
    [Authorize(Policy.ProjectAssignTasks)]
    public async Task<IActionResult> UpdateTaskAssignee(Guid taskId, UpdateTaskAssigneeDto model)
    {
        var result = await _mediator.Send(new UpdateTaskAssigneeCommand(taskId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/update-description")]
    [Authorize(Policy.ProjectAssignTasks)]
    public async Task<IActionResult> UpdateTaskDescription(Guid taskId, UpdateTaskDescriptionDto model)
    {
        var result = await _mediator.Send(new UpdateTaskDescriptionCommand(taskId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectAddComments)]
    public async Task<IActionResult> AddTaskComment(Guid taskId, AddTaskCommentDto model)
    {
        var result = await _mediator.Send(new AddTaskCommentCommand(taskId, User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }

    [HttpGet("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetTaskComments(Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskCommentsQuery(taskId));
        return result.ToHttpResult();
    }

    [HttpGet("{taskId:guid}/activities")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetTaskActivities(Guid taskId)
    {
        var result = await _mediator.Send(new GetTaskActivitiesQuery(taskId));
        return result.ToHttpResult();
    }
}
