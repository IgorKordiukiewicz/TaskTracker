using Domain.Common;
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
            result.Length.Should().Be(3);
            result[0].Permissions.Should().Be(EnumHelpers.GetAllFlags<ProjectPermissions>());
            result[0].Type.Should().Be(RoleType.Owner);
            result[1].Permissions.Should().Be(EnumHelpers.GetAllFlags<ProjectPermissions>());
            result[1].Type.Should().Be(RoleType.Admin);
            result[2].Permissions.Should().Be(ProjectPermissions.None);
            result[2].Type.Should().Be(RoleType.ReadOnly);
            result.All(x => x.ProjectId == projectId).Should().BeTrue();
        }
    }
}
