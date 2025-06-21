using Domain.Tasks;

namespace UnitTests.Domain;

public class TaskRelationManagerTests
{
    [Fact]
    public void AddHierarchicalRelation_ShouldAddNewRelation_WhenValidationSucceeds()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationManager.AddHierarchicalRelation(parentId, childId, [parentId, childId]);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            relationManager.HierarchicalRelations.Should().HaveCount(1);
        }
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenParentIsTheSameAsChild()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());
        var taskId = Guid.NewGuid();

        var result = relationManager.AddHierarchicalRelation(taskId, taskId, [taskId]);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenParentTaskDoesNotBelongToProject()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationManager.AddHierarchicalRelation(parentId, childId, [childId]);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenChildTaskDoesNotBelongToProject()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var result = relationManager.AddHierarchicalRelation(parentId, childId, [parentId]);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenChildAlreadyHasParent()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var existingParentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var newParentId = Guid.NewGuid();
        var projectTasksIds = new[] { existingParentId, childId, newParentId };
        _ = relationManager.AddHierarchicalRelation(existingParentId, childId, projectTasksIds);

        var result = relationManager.AddHierarchicalRelation(newParentId, childId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenReverseRelationAlreadyExists()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var projectTasksIds = new[] { parentId, childId };
        _ = relationManager.AddHierarchicalRelation(parentId, childId, projectTasksIds);

        var result = relationManager.AddHierarchicalRelation(childId, parentId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddHierarchicalRelation_ShouldFail_WhenRelationAlreadyExists()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var projectTasksIds = new[] { parentId, childId };
        _ = relationManager.AddHierarchicalRelation(parentId, childId, projectTasksIds);

        var result = relationManager.AddHierarchicalRelation(parentId, childId, projectTasksIds);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveHierarchicalRelation_ShouldFail_WhenRelationDoesNotExist()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var result = relationManager.RemoveHierarchicalRelation(Guid.NewGuid(), Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveHierarchicalRelation_ShouldRemoveRelation_WhenRelationExists()
    {
        var relationManager = new TaskRelationManager(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var projectTasksIds = new[] { parentId, childId };
        _ = relationManager.AddHierarchicalRelation(parentId, childId, projectTasksIds);

        var result = relationManager.RemoveHierarchicalRelation(parentId, childId);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            relationManager.HierarchicalRelations.Should().BeEmpty();
        }
    }
}
