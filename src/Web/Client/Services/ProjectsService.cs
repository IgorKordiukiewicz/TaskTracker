using Flurl;
using MudBlazor;
using Shared.Dtos;
using Shared.Enums;
using Shared.ViewModels;

namespace Web.Client.Services;

public class ProjectsService : ApiService
{
    public ProjectsService(HttpClient httpClient, ISnackbar snackbar) 
        : base(httpClient, snackbar)
    {
    }

    public async Task<ProjectsVM?> GetForOrganizationUser(Guid organizationId)
        => await Get<ProjectsVM>($"projects/organization/{organizationId}/user");

    public async Task<bool> Create(Guid organizationId, CreateProjectDto model)
        => await Post($"projects/organization/{organizationId}", model);

    public async Task<UsersSearchVM?> GetAvailableUsers(Guid projectId, Guid organizationId)
    {
        var url = "users/available-for-project"
                .SetQueryParam("projectId", projectId)
                .SetQueryParam("organizationId", organizationId)
                .ToString();
        return await Get<UsersSearchVM>(url);
    }

    public async Task<RolesVM<ProjectPermissions>?> GetRoles(Guid projectId)
        => await Get<RolesVM<ProjectPermissions>>($"projects/{projectId}/roles");

    public async Task<ProjectMembersVM?> GetMembers(Guid projectId)
        => await Get<ProjectMembersVM>($"projects/{projectId}/members");

    public async Task<bool> AddMember(Guid projectId, AddProjectMemberDto model)
        => await Post($"projects/{projectId}/members", model);

    public async Task<ProjectOrganizationVM?> GetOrganization(Guid projectId)
        => await Get<ProjectOrganizationVM>($"projects/{projectId}/organization");

    public async Task<ProjectSettingsVM?> GetSettings(Guid projectId)
        => await Get<ProjectSettingsVM>($"projects/{projectId}/settings");

    public async Task<bool> UpdateName(Guid projectId, UpdateProjectNameDto model)
        => await Post($"projects/{projectId}/update-name", model);

    public async Task<bool> Delete(Guid projectId)
        => await Post($"projects/{projectId}/delete");
}
