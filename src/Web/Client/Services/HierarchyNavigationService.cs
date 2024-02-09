using MudBlazor;
using Shared.ViewModels;

namespace Web.Client.Services;

public class HierarchyNavigationService
{
    private NavigationItemVM? _organization;
    private NavigationItemVM? _project;

    private readonly OrganizationsService _organizationsService;
    private readonly ProjectsService _projectsService;

    public event Action? Updated;

    public Guid? OrganizationId => _organization?.Id;
    public Guid? ProjectId => _project?.Id;

    public HierarchyNavigationService(OrganizationsService organizationsService, ProjectsService projectsService)
    {
        _organizationsService = organizationsService;
        _projectsService = projectsService;
    }

    public void OpenIndexPage()
    {
        _organization = null;
        _project = null;

        Updated?.Invoke();
    }

    public async Task OpenOrganizationPage(Guid organizationId)
    {
        if(OrganizationId == organizationId && ProjectId is null) // Only ignore when going from org page to org page, not when from org's project page to org page.
        {
            return;
        }

        _project = null;
        var navData = await _organizationsService.GetNavData(organizationId);
        _organization = navData?.Organization;

        Updated?.Invoke();
    }

    public async Task OpenProjectPage(Guid projectId)
    {
        if(ProjectId == projectId)
        {
            return;
        }

        var navData = await _projectsService.GetNavData(projectId);
        _project = navData?.Project;
        _organization = navData?.Organization;

        Updated?.Invoke();
    }

    public List<BreadcrumbItem> GetBreadcrumbs()
    {
        var result = new List<BreadcrumbItem>();

        if (_organization is not null)
        {
            result.Add(new(_organization.Name, href: $"/org/{_organization.Id}/projects"));
        }

        if (_project is not null)
        {
            result.Add(new(_project.Name, href: $"project/{_project.Id}/tasks/table"));
        }

        return result;
    }
}
