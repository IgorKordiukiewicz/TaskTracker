using Domain.Tasks;

namespace UnitTests.Domain;

public class TaskRelationshipManagerTests
{
    [Fact]
    public void AddHierarchicalRelationship_ShouldAddNewRelationship_WhenValidationSucceeds()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationshipManager.AddHierarchicalRelationship(parentId, childId, new[] { parentId, childId });

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            relationshipManager.HierarchicalRelationships.Should().HaveCount(1);
        }
    }

    [Fact]
    public void AddHierarchicalRelationship_ShouldFail_WhenParentTaskDoesNotBelongToProject()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationshipManager.AddHierarchicalRelationship(parentId, childId, new[] { childId });

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelationship_ShouldFail_WhenChildTaskDoesNotBelongToProject()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationshipManager.AddHierarchicalRelationship(parentId, childId, new[] { parentId });

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelationship_ShouldFail_WhenChildAlreadyHasParent()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var existingParentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var newParentId = Guid.NewGuid();
        var projectTasksIds = new[] { existingParentId, childId, newParentId };
        _ = relationshipManager.AddHierarchicalRelationship(existingParentId, childId, projectTasksIds);

        var result = relationshipManager.AddHierarchicalRelationship(newParentId, childId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelationship_ShouldFail_WhenReverseRelationshipAlreadyExists()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var projectTasksIds = new[] { parentId, childId };
        _ = relationshipManager.AddHierarchicalRelationship(parentId, childId, projectTasksIds);

        var result = relationshipManager.AddHierarchicalRelationship(childId, parentId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelationship_ShouldFail_WhenRelationshipAlreadyExists()
    {
        var relationshipManager = new TaskRelationshipManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var projectTasksIds = new[] { parentId, childId };
        _ = relationshipManager.AddHierarchicalRelationship(parentId, childId, projectTasksIds);

        var result = relationshipManager.AddHierarchicalRelationship(parentId, childId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }
}
