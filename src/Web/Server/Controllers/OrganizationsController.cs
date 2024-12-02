using Application.Features.Organizations;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="mediator"></param>
[ApiController]
[Route("organizations")]
[Authorize]
[ProducesResponseType(200)]
[ProducesResponseType(400)]
[ProducesResponseType(500)]
[Produces("application/json")]
public class OrganizationsController(IMediator mediator) 
    : ControllerBase
{
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
        var result = await mediator.Send(new CreateOrganizationCommand(model, User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of organizations the user belongs to.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OrganizationsForUserVM), 200)]
    public async Task<IActionResult> GetOrganizations()
    {
        var result = await mediator.Send(new GetOrganizationsForUserQuery(User.GetUserId()));
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
        var result = await mediator.Send(new GetOrganizationNavDataQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a invitation to organization for a given user.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization or user not found.</response> 
    [HttpPost("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationEditMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, [FromBody] CreateOrganizationInvitationDto model)
    {
        var result = await mediator.Send(new CreateOrganizationInvitationCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get all invitations of an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/invitations")]
    [Authorize(Policy.OrganizationEditMembers)]
    [ProducesResponseType(typeof(OrganizationInvitationsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationInvitationsForOrganization(Guid organizationId)
    {
        var result = await mediator.Send(new GetOrganizationInvitationsQuery(organizationId));
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
        var result = await mediator.Send(new GetOrganizationInvitationsForUserQuery(User.GetUserId()));
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
        var result = await mediator.Send(new DeclineOrganizationInvitationCommand(invitationId));
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
        var result = await mediator.Send(new AcceptOrganizationInvitationCommand(invitationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Cancel organization invitation (by organization member).
    /// </summary>
    /// <param name="invitationId"></param>
    /// <response code="404">Organization with given invitation not found.</response> 
    [HttpPost("invitations/{invitationId:guid}/cancel")]
    [Authorize(Policy.OrganizationEditMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelOrganizationInvitation(Guid invitationId)
    {
        var result = await mediator.Send(new CancelOrganizationInvitationCommand(invitationId));
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
        var result = await mediator.Send(new GetOrganizationMembersQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Remove member from an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/members/remove")]
    [Authorize(Policy.OrganizationEditMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveOrganizationMember(Guid organizationId, [FromBody] RemoveOrganizationMemberDto model)
    {
        var result = await mediator.Send(new RemoveOrganizationMemberCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update role of an organization member.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/members/role")]
    [Authorize(Policy.OrganizationEditMembers)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMemberRole(Guid organizationId, [FromBody] UpdateMemberRoleDto model)
    {
        var result = await mediator.Send(new UpdateOrganizationMemberRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get a list of roles for an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationMember)]
    [ProducesResponseType(typeof(RolesVM<OrganizationPermissions>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationRoles(Guid organizationId)
    {
        var result = await mediator.Send(new GetOrganizationRolesQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Create a new organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles")]
    [Authorize(Policy.OrganizationEditRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateOrganizationRole(Guid organizationId, [FromBody] CreateRoleDto<OrganizationPermissions> model)
    {
        var result = await mediator.Send(new CreateOrganizationRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/delete")]
    [Authorize(Policy.OrganizationEditRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrganizationRole(Guid organizationId, DeleteRoleDto model)
    {
        var result = await mediator.Send(new DeleteOrganizationRoleCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update name of an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/name")]
    [Authorize(Policy.OrganizationEditRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRoleName(Guid organizationId, [FromBody] UpdateRoleNameDto model)
    {
        var result = await mediator.Send(new UpdateOrganizationRoleNameCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update permissions of an organization role.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpPost("{organizationId:guid}/roles/permissions")]
    [Authorize(Policy.OrganizationEditRoles)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRolePermissions(Guid organizationId, [FromBody] UpdateRolePermissionsDto<OrganizationPermissions> model)
    {
        var result = await mediator.Send(new UpdateOrganizationRolePermissionsCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get settings for an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response>
    [HttpGet("{organizationId:guid}/settings")]
    [Authorize(Policy.OrganizationEditOrganization)]
    [ProducesResponseType(typeof(OrganizationSettingsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrganizationSettings(Guid organizationId)
    {
        var result = await mediator.Send(new GetOrganizationSettingsQuery(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update an organization's name.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="model"></param>
    /// <response code="404">Organization not found.</response>
    [HttpPost("{organizationId:guid}/name")]
    [Authorize(Policy.OrganizationEditOrganization)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateOrganizationName(Guid organizationId, [FromBody] UpdateOrganizationNameDto model)
    {
        var result = await mediator.Send(new UpdateOrganizationNameCommand(organizationId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Delete an organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response>
    [HttpPost("{organizationId:guid}/delete")]
    [Authorize(Policy.OrganizationOwner)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrganization(Guid organizationId)
    {
        var result = await mediator.Send(new DeleteOrganizationCommand(organizationId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get user's permission in a given organization.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response>
    [HttpGet("{organizationId:guid}/permissions")]
    [Authorize(Policy.OrganizationMember)]
    [ProducesResponseType(typeof(UserOrganizationPermissionsVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserOrganizationPermissions(Guid organizationId)
    {
        var result = await mediator.Send(new GetUserOrganizationPermissionsQuery(User.GetUserId(), organizationId));
        return result.ToHttpResult();
    }
}
