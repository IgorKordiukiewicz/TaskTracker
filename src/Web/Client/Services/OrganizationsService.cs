using Flurl;
using MudBlazor;
using Shared.Dtos;
using Shared.Enums;
using Shared.ViewModels;

namespace Web.Client.Services;

public class OrganizationsService : ApiService
{
    public OrganizationsService(HttpClient httpClient, ISnackbar snackbar)
        : base(httpClient, snackbar)
    {
        
    }

    public async Task<OrganizationMembersVM?> GetMembers(Guid organizationId)
        => await Get<OrganizationMembersVM>($"organizations/{organizationId}/members");

    public async Task<OrganizationInvitationsVM?> GetInvitations(Guid organizationId, Pagination pagination)
    {
        var url = $"organizations/{organizationId}/invitations"
            .SetQueryParam("pageNumber", pagination.PageNumber)
            .SetQueryParam("itemsPerPage", pagination.ItemsPerPage)
            .ToString();
        return await Get<OrganizationInvitationsVM>(url);
    }

    public async Task<RolesVM<OrganizationPermissions>?> GetRoles(Guid organizationId)
        => await Get<RolesVM<OrganizationPermissions>>($"organizations/{organizationId}/roles");

    public async Task<bool> Create(CreateOrganizationDto model)
        => await Post("organizations", model);

    public async Task<bool> SendInvitation(Guid organizationId, CreateOrganizationInvitationDto model)
        => await Post($"organizations/{organizationId}/invitations", model);

    public async Task<bool> CancelInvitation(Guid organizationId, Guid invitationId)
        => await Post($"organizations/invitations/{invitationId}/cancel", 
            headers: Headers.From(("OrganizationId", organizationId.ToString())));

    public async Task<bool> AcceptInvitation(Guid invitationId)
        => await Post($"organizations/invitations/{invitationId}/accept");

    public async Task<bool> DeclineInvitation(Guid invitationId)
        => await Post($"organizations/invitations/{invitationId}/decline");

    public async Task<OrganizationsForUserVM?> GetForUser()
        => await Get<OrganizationsForUserVM>("organizations/user");

    public async Task<UserOrganizationInvitationsVM?> GetInvitationsForUser()
        => await Get<UserOrganizationInvitationsVM>("organizations/invitations/user");

    public async Task<bool> RemoveMember(Guid organizationId, Guid memberId)
        => await Post($"organizations/{organizationId}/members/{memberId}/remove");

    public async Task<bool> UpdateMemberRole(Guid organizationId, Guid memberId, UpdateMemberRoleDto model)
        => await Post($"organizations/{organizationId}/members/{memberId}/update-role", model);

    public async Task<bool> UpdateRoleName(Guid organizationId, Guid roleId, UpdateRoleNameDto model)
        => await Post($"organizations/{organizationId}/roles/{roleId}/update-name", model);

    public async Task<bool> UpdateRolePermissions(Guid organizationId, Guid roleId, UpdateRolePermissionsDto<OrganizationPermissions> model)
        => await Post($"organizations/{organizationId}/roles/{roleId}/update-permissions", model);

    public async Task<bool> CreateRole(Guid organizationId, CreateRoleDto<OrganizationPermissions> model)
        => await Post($"organizations/{organizationId}/roles", model);

    public async Task<bool> DeleteRole(Guid organizationId, Guid roleId)
        => await Post($"organizations/{organizationId}/roles/{roleId}/delete");

    public async Task<OrganizationNavigationVM?> GetNavData(Guid organizationId)
        => await Get<OrganizationNavigationVM>($"organizations/{organizationId}/nav-data");
}