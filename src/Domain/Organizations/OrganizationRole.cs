using Domain.Common;
using Shared.Enums;

namespace Domain.Organizations;

public class OrganizationRole : Role<OrganizationPermissions>
{
    public Guid OrganizationId { get; private set; }

    public OrganizationRole(string name, Guid organizationId, OrganizationPermissions permissions) 
        : base(name, permissions)
    {
        OrganizationId = organizationId;
    }
}
