using AutoFixture.Kernel;
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
    public void RemovePermission_ShouldNotChangePermissionsValue_WhenGivenPermissionWasNotAlreadyAdded()
    {
        var role = new TestRole("role", TestPermissions.A);

        role.RemovePermission(TestPermissions.B);

        role.Permissions.Should().Be(TestPermissions.A);
    }

    [Fact]
    public void UpdateName_ShouldUpdateName()
    {
        var role = new TestRole("role", TestPermissions.A);
        var newName = "newName";

        role.UpdateName(newName);

        role.Name.Should().Be(newName);
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
    public void RolesManager_GetAdminRoleId_ShouldReturnIdOfTheAdminRole()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var adminRoleId = roles.Single(x => x.Type == RoleType.Admin).Id;

        var result = rolesManager.GetAdminRoleId();

        result.Should().Be(adminRoleId);
    }

    [Fact]
    public void RolesManager_GetReadOnlyRoleId_ShouldReturnIdOfTheReadOnlyRole()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var readOnlyRoleId = roles.Single(x => x.Type == RoleType.ReadOnly).Id;

        var result = rolesManager.GetReadOnlyRoleId();

        result.Should().Be(readOnlyRoleId);
    }

    [Fact]
    public void RolesManager_AddRole_ShouldFail_WhenGivenNameIsTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var result = rolesManager.AddRole(roles[0].Name, TestPermissions.A);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_AddRole_ShouldSucceedAndAppendNewRole_WhenGivenNameIsNotTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var rolesCountBefore = roles.Count;
        var result = rolesManager.AddRole("abc", TestPermissions.A);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.Count.Should().Be(rolesCountBefore + 1);
        }
    }

    [Fact]
    public void RolesManager_DeleteRole_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.DeleteRole(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_DeleteRole_ShouldFail_WhenRoleIsNotModifiable()
    {
        var roles = CreateTestRoles();
        var notModifiableRole = roles.First(x => x.Type != RoleType.Custom);
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.DeleteRole(notModifiableRole.Id);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_DeleteRole_ShouldDeleteRole_WhenRoleIsModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleName = "abc";
        _ = rolesManager.AddRole(roleName, TestPermissions.A);
        var rolesCountBefore = roles.Count;
        var roleToDelete = roles.First(x => x.Name == roleName);

        var result = rolesManager.DeleteRole(roleToDelete.Id);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.Count.Should().Be(rolesCountBefore - 1);
        }
    }

    [Fact]
    public void RolesManager_UpdateRoleName_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.UpdateRoleName(Guid.NewGuid(), "abc");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_UpdateRoleName_ShouldFail_WhenRoleIsNotModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleId = roles.First(x => !x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, "abc");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_UpdateRoleName_ShouldFail_WhenNameIsTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var name = roles.First(x => !x.IsModifiable()).Name;
        var roleId = roles.First(x => x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, name);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RolesManager_UpdateRoleName_ShouldUpdateRoleName_WhenNameIsNotTakenAndRoleIsModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var name = "abc";
        var roleId = roles.First(x => x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, name);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.First(x => x.Id == roleId).Name.Should().Be(name);
        }
    }

    private static List<TestRole> CreateTestRoles()
        => new()
        {
            new("a", default, RoleType.Admin),
            new("b", default, RoleType.Custom),
            new("c", default, RoleType.Custom),
            new("d", default, RoleType.ReadOnly),
        };

    private static RolesManager<TestRole, TestPermissions> CreateRolesManager(List<TestRole> roles)
        => new(roles, (name, permissions) => new TestRole(name, permissions));
}
