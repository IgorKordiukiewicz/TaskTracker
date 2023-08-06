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

    [HttpGet("is-registered/{userAuthenticationId}")]
    public async Task<IActionResult> IsUserRegistered(string userAuthenticationId)
    {
        var result = await _mediator.Send(new IsUserRegisteredQuery(userAuthenticationId));
        return result.ToHttpResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto model)
    {
        var result = await _mediator.Send(new RegisterUserCommand(model));
        return result.ToHttpResult(201);
    }

    [HttpGet("not-in-org")]
    public async Task<IActionResult> GetUsersNotInOrganization(Guid organizationId, string searchValue)
    {
        var result = await _mediator.Send(new GetUsersNotInOrganizationQuery(organizationId, searchValue));
        return result.ToHttpResult();
    }
}
