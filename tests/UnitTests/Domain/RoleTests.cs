using Domain.Common;
using Domain.Organizations;
using Domain.Projects;

namespace UnitTests.Domain;

public class RoleTests
{
    [Theory]
    [InlineData(TestPermissions.None, TestPermissions.A, false)]
    [InlineData(TestPermissions.A, TestPermissions.A, true)]
    [InlineData(TestPermissions.A | TestPermissions.B, TestPermissions.A, true)]
    [InlineData(TestPermissions.A | TestPermissions.B, TestPermissions.C, false)]
    public void HasPermission_ShouldReturnWhetherRoleHasOneOfThePermissions(TestPermissions permissions, TestPermissions flag, bool expected)
    {
        var role = new TestRole("role", permissions);

        var result = role.HasPermission(flag);

        result.Should().Be(expected);
    }

    [Fact]
    public void ProjectRole_CreateDefaultRoles_ShouldReturnAdminAndReadOnlyRole()
    {
        var projectId = Guid.NewGuid();
        var result = ProjectRole.CreateDefaultRoles(projectId);

        using(new AssertionScope())
        {
            result.Length.Should().Be(2);
            result[0].Permissions.Should().Be(EnumHelpers.GetAllFlags<ProjectPermissions>());
            result[0].Type.Should().Be(RoleType.Admin);
            result[1].Permissions.Should().Be(ProjectPermissions.None);
            result[1].Type.Should().Be(RoleType.ReadOnly);
            result.All(x => x.ProjectId == projectId).Should().BeTrue();
        }
    }

    [Fact]
    public void OrganizationRole_CreateDefaultRoles_ShouldReturnAdminAndReadOnlyRole()
    {
        var organizationId = Guid.NewGuid();
        var result = OrganizationRole.CreateDefaultRoles(organizationId);

        using (new AssertionScope())
        {
            result.Length.Should().Be(2);
            result[0].Permissions.Should().Be(EnumHelpers.GetAllFlags<OrganizationPermissions>());
            result[0].Type.Should().Be(RoleType.Admin);
            result[1].Permissions.Should().Be(OrganizationPermissions.None);
            result[1].Type.Should().Be(RoleType.ReadOnly);
            result.All(x => x.OrganizationId == organizationId).Should().BeTrue();
        }
    }
}
