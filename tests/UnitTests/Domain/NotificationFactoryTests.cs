using Domain.Notifications;

namespace UnitTests.Domain;

public class NotificationFactoryTests
{
    [Fact]
    public void TaskAssigned_ShouldCreateWithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskShortId = 1;

        var result = NotificationFactory.TaskAssigned(userId, projectId, taskShortId);

        using(new AssertionScope())
        {
            result.UserId.Should().Be(userId);
            result.Context.Should().Be(NotificationContext.Project);
            result.ContextEntityId.Should().Be(projectId);
            result.TaskShortId.Should().Be(taskShortId);
        }
    }

    [Fact]
    public void TaskUnassigned_ShouldCreateWithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskShortId = 1;

        var result = NotificationFactory.TaskUnassigned(userId, projectId, taskShortId);

        using (new AssertionScope())
        {
            result.UserId.Should().Be(userId);
            result.Context.Should().Be(NotificationContext.Project);
            result.ContextEntityId.Should().Be(projectId);
            result.TaskShortId.Should().Be(taskShortId);
        }
    }

    [Fact]
    public void AddedToProject_ShouldCreateWithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var result = NotificationFactory.AddedToProject(userId, projectId);

        using (new AssertionScope())
        {
            result.UserId.Should().Be(userId);
            result.Context.Should().Be(NotificationContext.Project);
            result.ContextEntityId.Should().Be(projectId);
        }
    }

    [Fact]
    public void RemovedFromProject_ShouldCreateWithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var result = NotificationFactory.RemovedFromProject(userId, projectId);

        using (new AssertionScope())
        {
            result.UserId.Should().Be(userId);
            result.Context.Should().Be(NotificationContext.Project);
            result.ContextEntityId.Should().Be(projectId);
        }
    }

    [Fact]
    public void RemovedFromOrganization_ShouldCreateWithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        var result = NotificationFactory.RemovedFromOrganization(userId, organizationId);

        using (new AssertionScope())
        {
            result.UserId.Should().Be(userId);
            result.Context.Should().Be(NotificationContext.Organization);
            result.ContextEntityId.Should().Be(organizationId);
        }
    }
}
