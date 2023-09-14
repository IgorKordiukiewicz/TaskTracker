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
            todoStatus.IsInitial.Should().BeTrue();

            var inProgressStatus = result.Statuses.First(x => x.Name.ToLower() == "inprogress");
            inProgressStatus.IsInitial.Should().BeFalse();

            var doneStatus = result.Statuses.First(x => x.Name.ToLower() == "done");
            doneStatus.IsInitial.Should().BeFalse();

            // Transitions
            todoStatus.CanTransitionTo(inProgressStatus.Id).Should().BeTrue();
            todoStatus.CanTransitionTo(doneStatus.Id).Should().BeFalse();

            inProgressStatus.CanTransitionTo(todoStatus.Id).Should().BeTrue();
            inProgressStatus.CanTransitionTo(doneStatus.Id).Should().BeTrue();

            doneStatus.CanTransitionTo(todoStatus.Id).Should().BeFalse();
            doneStatus.CanTransitionTo(inProgressStatus.Id).Should().BeTrue();

            // Display orders
            result.Statuses.OrderBy(x => x.DisplayOrder).Select(x => x.Name.ToLower())
                .Should().BeEquivalentTo(new[] { "todo", "inprogress", "done" }, options => options.WithStrictOrdering());
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
