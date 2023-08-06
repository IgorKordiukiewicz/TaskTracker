using Application.Features.Organizations;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Web.Server.Extensions;

namespace Web.Server.Controllers;

[ApiController]
[Route("organizations")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationCommand(model));
        return result.ToHttpResult(); // TODO: Return 201 with ID
    }

    [HttpGet]
    public async Task<IActionResult> GetOrganizations()
    {
        var result = await _mediator.Send(new GetOrganizationsForUserQuery(User.GetUserAuthenticationId()));
        return result.ToHttpResult();
    }

    [HttpPost("invitations")]
    [Authorize("OrganizationMember")]
    public async Task<IActionResult> CreateOrganizationInvitation([FromBody] CreateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationInvitationCommand(model));
        return result.ToHttpResult();
    }

    [HttpGet("invitations/user")]
    public async Task<IActionResult> GetOrganizationInvitationsForUser()
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsForUserQuery(User.GetUserAuthenticationId()));
        return result.ToHttpResult();
    }

    [HttpPost("invitations/decline")]
    public async Task<IActionResult> DeclineOrganizationInvitation([FromBody] UpdateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new DeclineOrganizationInvitationCommand(model));
        return result.ToHttpResult();
    }

    [HttpPost("invitations/accept")]
    public async Task<IActionResult> AcceptOrganizationInvitation([FromBody] UpdateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new AcceptOrganizationInvitationCommand(model));
        return result.ToHttpResult();
    }
}
