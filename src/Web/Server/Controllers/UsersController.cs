using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Web.Server.Extensions;

namespace Web.Server.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userAuthenticationId}/is-registered")]
    public async Task<IActionResult> IsUserRegistered(string userAuthenticationId)
    {
        var result = await _mediator.Send(new IsUserRegisteredQuery(userAuthenticationId));
        return result.ToHttpResult();
    }

    [HttpGet("{userAuthenticationId}/data")] // without /data the endpoint is not called
    public async Task<IActionResult> GetUser(string userAuthenticationId)
    {
        var result = await _mediator.Send(new GetUserQuery(userAuthenticationId));
        return result.ToHttpResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto model)
    {
        var result = await _mediator.Send(new RegisterUserCommand(model));
        return result.ToHttpResult(201);
    }

    [HttpGet("available-for-invitation/organization/{organizationId:guid}")]
    [Authorize(Policy.OrganizationMember)]
    public async Task<IActionResult> GetUsersNotInOrganization(Guid organizationId, [FromQuery] string searchValue) // TODO: OrganizationId from query?
    {
        var result = await _mediator.Send(new GetUsersAvailableForOrganizationInvitationQuery(organizationId, searchValue));
        return result.ToHttpResult();
    }

    [HttpGet("available-for-project/project/{projectId:guid}")]
    [Authorize(Policy.ProjectMember)]
    public async Task<IActionResult> GetUsersAvailableForProject(Guid projectId, [FromQuery] Guid organizationId)
    {
        var result = await _mediator.Send(new GetUsersAvailableForProjectQuery(organizationId, projectId));
        return result.ToHttpResult();
    }
}
