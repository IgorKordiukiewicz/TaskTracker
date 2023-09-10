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

    [HttpPost("{taskId:guid}/update-state/{newStateId:guid}")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> UpdateTaskState(Guid taskId, Guid newStateId)
    {
        var result = await _mediator.Send(new UpdateTaskStateCommand(taskId, newStateId));
        return result.ToHttpResult();
    }
}
