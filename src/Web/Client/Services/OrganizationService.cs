using Flurl;
using MudBlazor;
using Shared.Dtos;
using Shared.Enums;
using Shared.ViewModels;

namespace Web.Client.Services;

public class OrganizationService : ApiService
{
    public OrganizationService(HttpClient httpClient, ISnackbar snackbar)
        : base(httpClient, snackbar)
    {
        
    }

    public async Task<OrganizationMembersVM?> GetMembers(Guid organizationId)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Get, $"organizations/{organizationId}/members");
        return await GetResponseContent<OrganizationMembersVM>(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<OrganizationInvitationsVM?> GetInvitations(Guid organizationId, Pagination pagination)
    {
        var url = $"organizations/{organizationId}/invitations"
            .SetQueryParam("pageNumber", pagination.PageNumber)
            .SetQueryParam("itemsPerPage", pagination.ItemsPerPage)
            .ToString();
        var requestMessage = CreateRequestMessage(HttpMethod.Get, url);
        return await GetResponseContent<OrganizationInvitationsVM>(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<RolesVM<OrganizationPermissions>?> GetRoles(Guid organizationId)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Get, $"organizations/{organizationId}/roles");
        return await GetResponseContent<RolesVM<OrganizationPermissions>>(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<UsersSearchVM?> GetUsersAvailableForInvitation(Guid organizationId, string searchValue)
    {
        var url = $"users/available-for-invitation"
            .SetQueryParam("searchValue", searchValue)
            .SetQueryParam("organizationId", organizationId)
            .ToString();
        var requestMessage = CreateRequestMessage(HttpMethod.Get, url);
        return await GetResponseContent<UsersSearchVM>(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<bool> Create(CreateOrganizationDto model)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, "organizations");
        SetRequestMessageContent(requestMessage, model);
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<bool> SendInvitation(Guid organizationId, CreateOrganizationInvitationDto model)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, $"organizations/{organizationId}/invitations");
        SetRequestMessageContent(requestMessage, model);
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<bool> CancelInvitation(Guid organizationId, Guid invitationId)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, $"organizations/invitations/{invitationId}/cancel", 
            Headers.From(("OrganizationId", organizationId.ToString())));
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<bool> AcceptInvitation(Guid invitationId)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, $"organizations/invitations/{invitationId}/accept");
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<bool> DeclineInvitation(Guid invitationId)
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Post, $"organizations/invitations/{invitationId}/decline");
        return await GetPostResponseResult(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<OrganizationsForUserVM?> GetForUser()
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Get, $"organizations/user");
        return await GetResponseContent<OrganizationsForUserVM>(await _httpClient.SendAsync(requestMessage));
    }

    public async Task<UserOrganizationInvitationsVM?> GetInvitationsForUser()
    {
        var requestMessage = CreateRequestMessage(HttpMethod.Get, $"organizations/invitations/user");
        return await GetResponseContent<UserOrganizationInvitationsVM>(await _httpClient.SendAsync(requestMessage));
    }
}