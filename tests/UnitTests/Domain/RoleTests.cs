using Domain.Common;

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
    public TestRole(string name, TestPermissions permissions, bool modifiable = true) 
        : base(name, permissions, modifiable)
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
}
