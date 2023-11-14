﻿using Domain.Common;
using Domain.Projects;
using Shared.Enums;

namespace Domain.Organizations;

public class OrganizationRole : Role<OrganizationPermissions>
{
    public Guid OrganizationId { get; private set; }

    public OrganizationRole(string name, Guid organizationId, OrganizationPermissions permissions) 
        : this(name, organizationId, permissions, RoleType.Custom)
    {
    }

    public OrganizationRole(string name, Guid organizationId, OrganizationPermissions permissions, RoleType type)
        : base(name, permissions, type)
    {
        OrganizationId = organizationId;
    }

    public static OrganizationRole[] CreateDefaultRoles(Guid organizationId)
    {
        return new OrganizationRole[]
        {
            new("Administrator", organizationId, OrganizationPermissions.Projects | OrganizationPermissions.Members, RoleType.Admin),
            new("Read-Only", organizationId, OrganizationPermissions.None, RoleType.ReadOnly),
        };
    }
}