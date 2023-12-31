﻿using Application.Features.Organizations;

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
        return result.ToHttpResult();
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
    [Authorize(Policy.OrganizationInviteMembers)]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, [FromBody] CreateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationInvitationCommand(organizationId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationInviteMembers)]
    public async Task<IActionResult> GetOrganizationInvitationsForOrganization(Guid organizationId, [FromQuery] Pagination pagination)
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsQuery(organizationId, pagination));
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
    [Authorize(Policy.OrganizationInviteMembers)]
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
    [Authorize(Policy.OrganizationRemoveMembers)]
    public async Task<IActionResult> RemoveOrganizationMember(Guid organizationId, Guid memberId)
    {
        var result = await _mediator.Send(new RemoveOrganizationMemberCommand(organizationId, memberId));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/members/{memberId:guid}/update-role")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> UpdateMemberRole(Guid organizationId, Guid memberId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await _mediator.Send(new UpdateOrganizationMemberRoleCommand(organizationId, memberId, model));
        return result.ToHttpResult();
    }

    [HttpGet("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> GetOrganizationRoles(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationRolesQuery(organizationId));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> CreateOrganizationRole(Guid organizationId, [FromBody] CreateRoleDto<OrganizationPermissions> model)
    {
        var result = await _mediator.Send(new CreateOrganizationRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/roles/{roleId:guid}/delete")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> DeleteOrganizationRole(Guid organizationId, Guid roleId)
    {
        var result = await _mediator.Send(new DeleteOrganizationRoleCommand(organizationId, roleId));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/roles/{roleId:guid}/update-name")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> UpdateRoleName(Guid organizationId, Guid roleId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await _mediator.Send(new UpdateOrganizationRoleNameCommand(organizationId, roleId, model));
        return result.ToHttpResult();
    }

    [HttpPost("{organizationId:guid}/roles/{roleId:guid}/update-permissions")]
    [Authorize(Policy.OrganizationManageRoles)]
    public async Task<IActionResult> UpdateRolePermissions(Guid organizationId, Guid roleId, [FromBody] UpdateRolePermissionsDto<OrganizationPermissions> model)
    {
        var result = await _mediator.Send(new UpdateOrganizationRolePermissionsCommand(organizationId, roleId, model));
        return result.ToHttpResult();
    }
}
