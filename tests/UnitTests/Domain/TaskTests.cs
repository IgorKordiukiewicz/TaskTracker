using Task = Domain.Tasks.Task;

namespace UnitTests.Domain;

public class TaskTests
{
    [Fact]
    public void Create_ShouldCreateTask_WithGivenParameters()
    {
        var shortId = 1;
        var projectId = Guid.NewGuid();
        var title = "Title";
        var description = "Description";

        var result = Task.Create(shortId, projectId, title, description);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.ShortId.Should().Be(shortId);
            result.ProjectId.Should().Be(projectId);
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
        }
    }
}
