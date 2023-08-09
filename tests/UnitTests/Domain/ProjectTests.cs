using Domain.Projects;

namespace UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Create_ShouldCreateProjectWithGivenParameters()
    {
        var name = "project";
        var orgId = Guid.NewGuid();
        var result = Project.Create(name, orgId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(name);
            result.OrganizationId.Should().Be(orgId);
        }
    }

    [Fact]
    public void AddMember_ShouldAddNewMember()
    {
        var project = Project.Create("name", Guid.NewGuid());
        var userId = Guid.NewGuid();

        var result = project.AddMember(userId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.UserId.Should().Be(userId);
            project.Members.Count.Should().Be(1);
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
}
