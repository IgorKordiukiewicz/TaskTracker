using Application.Features.Organizations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
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

    [HttpGet("user")]
    public async Task<IActionResult> GetOrganizations()
    {
        var result = await _mediator.Send(new GetOrganizationsForUserQuery(User.GetUserAuthenticationId()));
        return result.ToHttpResult();
    }

    [HttpGet("{organizationId:guid}/nav-data")]
    public async Task<IActionResult> GetOrganizationNavData(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationNavDataQuery(organizationId));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationMembersEditor)]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, [FromBody] CreateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationInvitationCommand(organizationId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationMembersEditor)]
    public async Task<IActionResult> GetOrganizationInvitationsForOrganization(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsQuery(organizationId));
        return result.ToHttpResult();
    }

    [HttpGet("invitations/user")]
    public async Task<IActionResult> GetOrganizationInvitationsForUser()
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsForUserQuery(User.GetUserAuthenticationId()));
        return result.ToHttpResult();
    }

    [HttpPost("invitations/{invitationId:guid}/decline")]
    public async Task<IActionResult> DeclineOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new DeclineOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    [HttpPost("invitations/{invitationId:guid}/accept")]
    public async Task<IActionResult> AcceptOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new AcceptOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    [HttpPost("invitations/{invitationId:guid}/cancel")]
    [Authorize(Policy.OrganizationMembersEditor)]
    public async Task<IActionResult> CancelOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new CancelOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }


    [HttpGet("{organizationId:guid}/members")]
    [Authorize(Policy.OrganizationMember)]
    public async Task<IActionResult> GetOrganizationMembers(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationMembersQuery(organizationId));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/members/{memberId:guid}/remove")]
    [Authorize(Policy.OrganizationMembersEditor)]
    public async Task<IActionResult> RemoveOrganizationMember(Guid organizationId, Guid memberId)
    {
        var result = await _mediator.Send(new RemoveOrganizationMemberCommand(organizationId, memberId));
        return result.ToHttpResult();
    }
}
