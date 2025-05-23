using Domain.Projects;

namespace UnitTests.Domain;

public class MemberRoleTests
{
    [Theory]
    [InlineData(ProjectPermissions.None, ProjectPermissions.EditMembers, false)]
    [InlineData(ProjectPermissions.EditMembers, ProjectPermissions.EditMembers, true)]
    [InlineData(ProjectPermissions.EditMembers | ProjectPermissions.EditRoles, ProjectPermissions.EditMembers, true)]
    [InlineData(ProjectPermissions.EditMembers | ProjectPermissions.EditRoles, ProjectPermissions.EditProject, false)]
    public void HasPermission_ShouldReturnWhetherRoleHasOneOfThePermissions(ProjectPermissions permissions, ProjectPermissions flag, bool expected)
    {
        var role = new MemberRole("role", Guid.NewGuid(), permissions);

        var result = role.HasPermission(flag);

        result.Should().Be(expected);
    }

    [Fact]
    public void Only_Custom_Role_Is_Modifiable()
    {
        var customRole = new MemberRole("role", Guid.NewGuid(), ProjectPermissions.None);
        var defaultRoles = MemberRole.CreateDefaultRoles(Guid.NewGuid());

        using(new AssertionScope())
        {
            customRole.IsModifiable().Should().BeTrue();
            defaultRoles.Should().AllSatisfy(x => x.IsModifiable().Should().BeFalse());
        }
    }

    [Fact]
    public void ProjectRole_CreateDefaultRoles_ShouldReturnOwnerAndAdminAndReadOnlyRole()
    {
        var projectId = Guid.NewGuid();
        var result = MemberRole.CreateDefaultRoles(projectId);

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
