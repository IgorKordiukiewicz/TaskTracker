﻿using Domain.Common;

namespace UnitTests.Domain;

public class RolesManagerTests
{
    [Fact]
    public void GetAdminRoleId_ShouldReturnIdOfTheAdminRole()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var adminRoleId = roles.Single(x => x.Type == RoleType.Admin).Id;

        var result = rolesManager.GetAdminRoleId();

        result.Should().Be(adminRoleId);
    }

    [Fact]
    public void GetReadOnlyRoleId_ShouldReturnIdOfTheReadOnlyRole()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var readOnlyRoleId = roles.Single(x => x.Type == RoleType.ReadOnly).Id;

        var result = rolesManager.GetReadOnlyRoleId();

        result.Should().Be(readOnlyRoleId);
    }

    [Fact]
    public void AddRole_ShouldFail_WhenGivenNameIsTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var result = rolesManager.AddRole(roles[0].Name, TestPermissions.A);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddRole_ShouldSucceedAndAppendNewRole_WhenGivenNameIsNotTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var rolesCountBefore = roles.Count;
        var result = rolesManager.AddRole("abc", TestPermissions.A);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.Count.Should().Be(rolesCountBefore + 1);
        }
    }

    [Fact]
    public void DeleteRole_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.DeleteRole(Guid.NewGuid(), Array.Empty<IHasRole>());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeleteRole_ShouldFail_WhenRoleIsNotModifiable()
    {
        var roles = CreateTestRoles();
        var notModifiableRole = roles.First(x => x.Type != RoleType.Custom);
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.DeleteRole(notModifiableRole.Id, Array.Empty<IHasRole>());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeleteRole_ShouldFail_WhenRoleIsAssignedToMember()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleName = "abc";
        _ = rolesManager.AddRole(roleName, TestPermissions.A);
        var roleToDelete = roles.First(x => x.Name == roleName);
        var members = new List<TestMember>() { new(roleToDelete.Id) };

        var result = rolesManager.DeleteRole(roleToDelete.Id, members);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeleteRole_ShouldDeleteRole_WhenRoleIsModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleName = "abc";
        _ = rolesManager.AddRole(roleName, TestPermissions.A);
        var rolesCountBefore = roles.Count;
        var roleToDelete = roles.First(x => x.Name == roleName);

        var result = rolesManager.DeleteRole(roleToDelete.Id, Array.Empty<IHasRole>());

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.Count.Should().Be(rolesCountBefore - 1);
        }
    }

    [Fact]
    public void UpdateRoleName_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.UpdateRoleName(Guid.NewGuid(), "abc");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateRoleName_ShouldFail_WhenRoleIsNotModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleId = roles.First(x => !x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, "abc");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateRoleName_ShouldFail_WhenNameIsTaken()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var name = roles.First(x => !x.IsModifiable()).Name;
        var roleId = roles.First(x => x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, name);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateRoleName_ShouldUpdateRoleName_WhenNameIsNotTakenAndRoleIsModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var name = "abc";
        var roleId = roles.First(x => x.IsModifiable()).Id;

        var result = rolesManager.UpdateRoleName(roleId, name);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.First(x => x.Id == roleId).Name.Should().Be(name);
        }
    }

    [Fact]
    public void UpdateMemberRole_ShouldFail_WhenMemberDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.UpdateMemberRole(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<IHasRole>());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateMemberRole_ShouldFail_WhenCurrentRoleIsOwner()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var members = new List<TestMember>() { new(roles.First(x => x.Type == RoleType.Owner).Id) };

        var result = rolesManager.UpdateMemberRole(members[0].Id, roles[1].Id, members);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateMemberRole_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var members = new List<TestMember>() { new(Guid.NewGuid()) };

        var result = rolesManager.UpdateMemberRole(members[0].Id, Guid.NewGuid(), members);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateMemberRole_ShouldFail_WhenRoleIsOwnerRole()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var members = new List<TestMember>() { new(roles[0].Id) };

        var result = rolesManager.UpdateMemberRole(members[0].Id, roles.First(x => x.Type == RoleType.Owner).Id, members);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UpdateMemberRole_ShouldSucceed_WhenMemberAndRoleExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var members = new List<TestMember>() { new(roles[0].Id) };

        var result = rolesManager.UpdateMemberRole(members[0].Id, roles[1].Id, members);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UpdateRolePermissions_ShouldFail_WhenRoleDoesNotExist()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);

        var result = rolesManager.UpdateRolePermissions(Guid.NewGuid(), TestPermissions.A);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateRolePermissions_ShouldFail_WhenRoleIsNotModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleId = roles.First(x => !x.IsModifiable()).Id;

        var result = rolesManager.UpdateRolePermissions(roleId, TestPermissions.A);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateRolePermissions_ShouldUpdatePermissions_WhenRoleIsModifiable()
    {
        var roles = CreateTestRoles();
        var rolesManager = CreateRolesManager(roles);
        var roleId = roles.First(x => x.IsModifiable()).Id;
        var newPermissions = TestPermissions.A | TestPermissions.C;

        var result = rolesManager.UpdateRolePermissions(roleId, newPermissions);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            roles.First(x => x.Id == roleId).Permissions.Should().Be(newPermissions);
        }
    }

    private static List<TestRole> CreateTestRoles()
        => new()
        {
            new("a", default, RoleType.Admin),
            new("b", default, RoleType.Custom),
            new("c", default, RoleType.Custom),
            new("d", default, RoleType.ReadOnly),
            new("e", default, RoleType.Owner)
        };

    private static RolesManager<TestRole, TestPermissions> CreateRolesManager(List<TestRole> roles)
        => new(roles, (name, permissions) => new TestRole(name, permissions));
}
