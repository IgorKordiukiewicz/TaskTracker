﻿namespace Application.Models.ViewModels;

public record NavigationItemVM(Guid Id, string Name);

public record OrganizationNavigationVM(NavigationItemVM Organization);
public record ProjectNavigationVM(NavigationItemVM Project, NavigationItemVM Organization);
