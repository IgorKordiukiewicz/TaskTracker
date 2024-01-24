﻿using MudBlazor;
using Shared.ViewModels;
using Web.Client.Components;

namespace Web.Client.Services;

public class HierarchyNavigationService
{
    private NavigationItemVM? _organization = null;
    private NavigationItemVM? _project = null;

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

        if (Updated is not null)
        {
            Updated();
        }
    }

    public async Task OpenOrganizationPage(Guid organizationId)
    {
        _project = null;
        var navData = await _organizationsService.GetNavData(organizationId);
        _organization = navData?.Organization;

        if (Updated is not null)
        {
            Updated();
        }
    }

    public async Task OpenProjectPage(Guid projectId)
    {
        var navData = await _projectsService.GetNavData(projectId);
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

        if (_project is not null)
        {
            result.Add(new(_project.Name, href: $"project/{_project.Id}/"));
        }

        return result;
    }
}