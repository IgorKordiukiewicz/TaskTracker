using Application.Features.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Web.Server.Extensions;

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
    [Authorize(Policy.ProjectMember)]
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
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, Guid newStatusId)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(taskId, newStatusId));
        return result.ToHttpResult();
    }

    [HttpPost("{taskId:guid}/comments")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> AddTaskComment(Guid taskId, AddTaskCommentDto model)
    {
        var result = await _mediator.Send(new AddTaskCommentCommand(taskId, User.GetUserAuthenticationId(), model));
        return result.ToHttpResult();
    }
}
