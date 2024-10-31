using Application.Features.Organizations;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("organizations")]
[Authorize]
[ProducesResponseType(200)]
[ProducesResponseType(400)]
[ProducesResponseType(500)]
[Produces("application/json")]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new organization.
    /// </summary>
    /// <remarks>
    /// Returns ID of the created organization.
    /// </remarks>
    /// <param name="model"></param>
    /// <response code="404">User not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationCommand(model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of organizations the user belongs to.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OrganizationsForUserVM), 200)]
    public async Task<IActionResult> GetOrganizations()
    {
        var result = await _mediator.Send(new GetOrganizationsForUserQuery(User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get navigation data for organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/nav-data")]
    [ProducesResponseType(typeof(OrganizationNavigationVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationNavData(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationNavDataQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a invitation to organization for a given user.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization or user not found.</response> 
    [HttpPost("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationInviteMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, [FromBody] CreateOrganizationInvitationDto model)
    {
        var result = await _mediator.Send(new CreateOrganizationInvitationCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get all invitations of an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="pagination"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationInviteMembers)]
    [ProducesResponseType(typeof(OrganizationInvitationsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationInvitationsForOrganization(Guid organizationId, [FromQuery] Pagination pagination)
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsQuery(organizationId, pagination));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get pending invitations for the user.
    /// </summary>
    /// <response code="404">User not found.</response> 
    [HttpGet("invitations")]
    [ProducesResponseType(typeof(UserOrganizationInvitationsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationInvitationsForUser()
    {
        var result = await _mediator.Send(new GetOrganizationInvitationsForUserQuery(User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Decline organization invitation (by user).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Organization with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/decline")]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeclineOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new DeclineOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Accept organization invitation (by user).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Organization with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/accept")]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AcceptOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new AcceptOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Cancel organization invitation (by organization member).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Organization with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/cancel")]
    [Authorize(Policy.OrganizationInviteMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelOrganizationInvitation(Guid invitationId)
    {
        var result = await _mediator.Send(new CancelOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get members of an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/members")]
    [Authorize(Policy.OrganizationMember)]
    [ProducesResponseType(typeof(OrganizationMembersVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationMembers(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationMembersQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Remove member from an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/members/remove")]
    [Authorize(Policy.OrganizationRemoveMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveOrganizationMember(Guid organizationId, [FromBody] RemoveOrganizationMemberDto model)
    {
        var result = await _mediator.Send(new RemoveOrganizationMemberCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update role of an organization member.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="memberId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/members/role")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMemberRole(Guid organizationId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await _mediator.Send(new UpdateOrganizationMemberRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of roles for an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(typeof(RolesVM<OrganizationPermissions>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationRoles(Guid organizationId)
    {
        var result = await _mediator.Send(new GetOrganizationRolesQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a new organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateOrganizationRole(Guid organizationId, [FromBody] CreateRoleDto<OrganizationPermissions> model)
    {
        var result = await _mediator.Send(new CreateOrganizationRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/delete")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrganizationRole(Guid organizationId, DeleteRoleDto model)
    {
        var result = await _mediator.Send(new DeleteOrganizationRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update name of an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/name")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRoleName(Guid organizationId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await _mediator.Send(new UpdateOrganizationRoleNameCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update permissions of an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/permissions")]
    [Authorize(Policy.OrganizationManageRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRolePermissions(Guid organizationId, [FromBody] UpdateRolePermissionsDto<OrganizationPermissions> model)
    {
        var result = await _mediator.Send(new UpdateOrganizationRolePermissionsCommand(organizationId, model));
        return result.ToHttpResult();
    }
}
