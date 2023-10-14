using Domain.Tasks;

namespace UnitTests.Domain;

public class WorkflowTests
{
    [Fact]
    public void Create_ShouldWorkflow_WithDefaultStatuses()
    {
        var result = Workflow.Create(Guid.NewGuid());

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.Statuses.Should().HaveCount(3);

            var todoStatus = result.Statuses.First(x => x.Name.ToLower() == "todo");
            todoStatus.Initial.Should().BeTrue();

            var inProgressStatus = result.Statuses.First(x => x.Name.ToLower() == "in progress");
            inProgressStatus.Initial.Should().BeFalse();

            var doneStatus = result.Statuses.First(x => x.Name.ToLower() == "done");
            doneStatus.Initial.Should().BeFalse();

            // Transitions
            DoesTransitionExist(todoStatus.Id, inProgressStatus.Id).Should().BeTrue();
            DoesTransitionExist(inProgressStatus.Id, todoStatus.Id).Should().BeTrue();
            DoesTransitionExist(inProgressStatus.Id, doneStatus.Id).Should().BeTrue();
            DoesTransitionExist(doneStatus.Id, inProgressStatus.Id).Should().BeTrue();

            bool DoesTransitionExist(Guid fromStatusId, Guid toStatusId)
                => result!.Transitions.Any(x => x.FromStatusId == fromStatusId && x.ToStatusId == toStatusId);

            // Display orders
            result.Statuses.OrderBy(x => x.DisplayOrder).Select(x => x.Name.ToLower())
                .Should().BeEquivalentTo(new[] { "todo", "in progress", "done" }, options => options.WithStrictOrdering());
        }
    }

    [Fact]
    public void DoesStatusExist_ShouldReturnFalse_WhenStatusDoesNotExist()
    {
        var workflow = Workflow.Create(Guid.NewGuid());

        var result = workflow.DoesStatusExist(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public void DoesStatusExist_ShouldReturnTrue_WhenStatusExists()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var status = workflow.Statuses[0];

        var result = workflow.DoesStatusExist(status.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public void CanTransitionTo_ShouldReturnFalse_WhenCantTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var status = workflow.Statuses[0];

        var result = workflow.CanTransitionTo(status.Id, Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public void CanTransitionTo_ShouldReturnTrue_WhenCanTransitionToNewState()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var transition = workflow.Transitions[0];

        var result = workflow.CanTransitionTo(transition.FromStatusId, transition.ToStatusId);

        result.Should().BeTrue();
    }

    [Fact]
    public void AddStatus_ShouldFail_WhenNameIsTaken()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var status = workflow.Statuses[0];

        var result = workflow.AddStatus(status.Name);
        
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddStatus_ShouldAddNewStatusWithIncrementedDisplayOrder_WhenNameIsNotTaken()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var name = workflow.Statuses[0].Name + "1";
        var statusesCountBefore = workflow.Statuses.Count;
        var expectedDisplayOrder = statusesCountBefore;

        var result = workflow.AddStatus(name);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            workflow.Statuses.Count.Should().Be(statusesCountBefore + 1);
            var newStatus = workflow.Statuses.First(x => x.Name == name);
            newStatus.DisplayOrder.Should().Be(expectedDisplayOrder);
        }
    }

    [Fact]
    public void TaskStatus_CanTransitionTo_ReturnsWhetherStatusCanTransitionToGivenStatus()
    {
        var availableStatus = Guid.NewGuid();
        var unavailableStatus = Guid.NewGuid();

        var taskStatus = global::Domain.Tasks.TaskStatus.Create(Guid.NewGuid(), new("test"), new[] { availableStatus }, 0);

        using(new AssertionScope())
        {
            taskStatus.CanTransitionTo(availableStatus).Should().BeTrue();
            taskStatus.CanTransitionTo(unavailableStatus).Should().BeFalse();
        }
    }
}
