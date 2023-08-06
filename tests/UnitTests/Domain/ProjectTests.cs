using Domain.Projects;

namespace UnitTests.Domain;

public class ProjectTests
{
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
}
