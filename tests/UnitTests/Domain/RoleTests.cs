using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Shared.Enums;

namespace UnitTests.Domain;

[Flags]
public enum TestPermissions
{
    None = 0,
    A = 1 << 0, // 1
    B = 1 << 1, // 2
    C = 1 << 2, // 4
}

public class TestRole : Role<TestPermissions>
{
    public TestRole(string name, TestPermissions permissions, RoleType type = RoleType.Custom) 
        : base(name, permissions, type)
    {
    }
}

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
    public void AddPermission_ShouldAddANewPermission_WhenGivenPermissionWasNotAlreadyAdded()
    {
        var role = new TestRole("role", TestPermissions.A);

        role.AddPermission(TestPermissions.C);

        role.Permissions.Should().Be(TestPermissions.A | TestPermissions.C);
    }

    [Fact]
    public void AddPermission_ShouldNotChangePermissionsValue_WhenGivenPermissionWasAlreadyAdded()
    {
        var role = new TestRole("role", TestPermissions.A);

        role.AddPermission(TestPermissions.A);

        role.Permissions.Should().Be(TestPermissions.A);
    }

    [Fact]
    public void RemovePermission_ShouldRemovePermission_WhenGivenPermissionWasAlreadyAdded()
    {
        var role = new TestRole("role", TestPermissions.A | TestPermissions.B);

        role.RemovePermission(TestPermissions.A);

        role.Permissions.Should().Be(TestPermissions.B);
    }

    [Fact]
    public void RemovePermission_ShouldRNotChangePermissionsValue_WhenGivenPermissionWasNotAlreadyAdded()
    {
        var role = new TestRole("role", TestPermissions.A);

        role.RemovePermission(TestPermissions.B);

        role.Permissions.Should().Be(TestPermissions.A);
    }

    [Fact]
    public void ProjectRole_CreateDefaultRoles_ShouldReturnAdminAndReadOnlyRole()
    {
        var projectId = Guid.NewGuid();
        var result = ProjectRole.CreateDefaultRoles(projectId);

        using(new AssertionScope())
        {
            result.Length.Should().Be(2);
            result[0].Permissions.Should().Be(ProjectPermissions.Members | ProjectPermissions.Workflows | ProjectPermissions.Tasks);
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
            result[0].Permissions.Should().Be(OrganizationPermissions.Projects | OrganizationPermissions.Members);
            result[0].Type.Should().Be(RoleType.Admin);
            result[1].Permissions.Should().Be(OrganizationPermissions.None);
            result[1].Type.Should().Be(RoleType.ReadOnly);
            result.All(x => x.OrganizationId == organizationId).Should().BeTrue();
        }
    }

    [Fact]
    public void RolesExtensions_GetAdminRoleId_ShouldReturnIdOfTheAdminRole()
    {
        var roles = CreateTestRoles();
        var adminRoleId = roles.Single(x => x.Type == RoleType.Admin).Id;

        var result = roles.GetAdminRoleId();

        result.Should().Be(adminRoleId);
    }

    [Fact]
    public void RolesExtensions_GetReadOnlyRoleId_ShouldReturnIdOfTheReadOnlyRole()
    {
        var roles = CreateTestRoles();
        var readOnlyRoleId = roles.Single(x => x.Type == RoleType.ReadOnly).Id;

        var result = roles.GetReadOnlyRoleId();

        result.Should().Be(readOnlyRoleId);
    }

    private static List<TestRole> CreateTestRoles()
        => new()
        {
            new("a", default, RoleType.Admin),
            new("b", default, RoleType.Custom),
            new("c", default, RoleType.Custom),
            new("d", default, RoleType.ReadOnly),
        };
}
