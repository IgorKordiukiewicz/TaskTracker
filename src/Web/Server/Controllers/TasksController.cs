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
    [Authorize("ProjectMember")]
    public async Task<IActionResult> CreateTask([FromHeader] Guid projectId, [FromBody] CreateTaskDto model)
    {
        var result = await _mediator.Send(new CreateTaskCommand(projectId, model));
        return result.ToHttpResult();
    }
}
