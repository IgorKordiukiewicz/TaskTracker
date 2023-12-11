using MudBlazor;
using Shared.ViewModels;
using Web.Client.Components;

namespace Web.Client.Common;

public class HierarchyNavigationService
{
    private NavigationItemVM? _organization = null;
    private NavigationItemVM? _project = null;

    private readonly RequestHandler _requestHandler;

    public event Action? Updated;

    public Guid? OrganizationId => _organization?.Id;
    public Guid? ProjectId => _project?.Id;

    public HierarchyNavigationService(RequestHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }

    public void OpenIndexPage()
    {
        _organization = null;
        _project = null;

        if (Updated is not null)
        {
            Updated();
        }
    }

    public async Task OpenOrganizationPage(Guid organizationId)
    {
        _project = null;
        var navData = await _requestHandler.GetAsync<OrganizationNavigationVM>($"organizations/{organizationId}/nav-data");
        _organization = navData?.Organization;

        if(Updated is not null)
        {
            Updated();
        }
    }

    public async Task OpenProjectPage(Guid projectId)
    {
        var navData = await _requestHandler.GetAsync<ProjectNavigationVM>($"projects/{projectId}/nav-data");
        _project = navData?.Project;
        _organization = navData?.Organization;

        if (Updated is not null)
        {
            Updated();
        }
    }

    public List<BreadcrumbItem> GetBreadcrumbs()
    {
        var result = new List<BreadcrumbItem>();

        if (_organization is not null)
        {
            result.Add(new(_organization.Name, href: $"/org/{_organization.Id}/"));
        }

        if(_project is not null)
        {
            result.Add(new(_project.Name, href: $"project/{_project.Id}/"));
        }

        return result;
    }
}
