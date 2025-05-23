using Domain.Common;
using Domain.Projects;

namespace UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Create_ShouldCreateProjectWithOwnerMemberAndDefaultRoles()
    {
        var name = "project";
        var userId = Guid.NewGuid();
        var result = Project.Create(name, userId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(name);

            // Ensure default roles are created
            result.Roles.Count.Should().Be(2);
            result.Roles.Select(x => x.Type).Should().BeEquivalentTo(new RoleType[] { RoleType.Admin, RoleType.ReadOnly });
            var adminRoleId = result.Roles.First(x => x.Type == RoleType.Admin).Id;

            result.Members.Count.Should().Be(1);
            result.Members[0].UserId.Should().Be(userId);
            result.Members[0].RoleId.Should().Be(adminRoleId);
        }
    }

    [Fact]
    public void AddMember_ShouldAddNewMember()
    {
        var project = Project.Create("name", Guid.NewGuid());
        var expectedRoleId = project.Roles.First(x => x.Type == RoleType.ReadOnly).Id;
        var userId = Guid.NewGuid();

        var result = project.AddMember(userId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.UserId.Should().Be(userId);
            result.Value.RoleId.Should().Be(expectedRoleId);
            project.Members.Count.Should().Be(2);
        }
    }

    [Fact]
    public void AddMember_ShouldFail_WhenUserIsAlreadyAMember()
    {
        var project = Project.Create("name", Guid.NewGuid());
        var userId = Guid.NewGuid();
        _ = project.AddMember(userId);

        var result = project.AddMember(userId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldFail_WhenMemberDoesNotExist()
    {
        var project = Project.Create("name", Guid.NewGuid());

        var result = project.RemoveMember(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMember_WhenMemberExists()
    {
        var project = Project.Create("name", Guid.NewGuid());

        var result = project.RemoveMember(project.Members[0].Id);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            project.Members.Count.Should().Be(0);
        }
    }

    [Fact]
    public void Member_UpdateRole_ShouldUpdateRoleId()
    {
        var member = ProjectMember.Create(Guid.NewGuid(), Guid.NewGuid());
        var newRoleId = Guid.NewGuid();

        member.UpdateRole(newRoleId);

        member.RoleId.Should().Be(newRoleId);
    }
}
