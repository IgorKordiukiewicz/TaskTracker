using Domain.Tasks;

namespace UnitTests.Domain;

public class TaskStatesManagerTests
{
    [Fact]
    public void Create_ShouldCreateTaskStatesManager_WithDefaultStates()
    {
        var result = TaskStatesManager.Create(Guid.NewGuid());

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.AllStates.Should().HaveCount(3);

            var todoState = result.AllStates.First(x => x.Name.Value.ToLower() == "todo");
            todoState.IsInitial.Should().BeTrue();

            var inProgressState = result.AllStates.First(x => x.Name.Value.ToLower() == "inprogress");
            inProgressState.IsInitial.Should().BeFalse();

            var doneState = result.AllStates.First(x => x.Name.Value.ToLower() == "done");
            doneState.IsInitial.Should().BeFalse();

            // Transitions
            todoState.CanTransitionTo(inProgressState.Id).Should().BeTrue();
            todoState.CanTransitionTo(doneState.Id).Should().BeFalse();

            inProgressState.CanTransitionTo(todoState.Id).Should().BeTrue();
            inProgressState.CanTransitionTo(doneState.Id).Should().BeTrue();

            doneState.CanTransitionTo(todoState.Id).Should().BeFalse();
            doneState.CanTransitionTo(inProgressState.Id).Should().BeTrue();

            // Display orders
            result.AllStates.OrderBy(x => x.DisplayOrder).Select(x => x.Name.Value.ToLower())
                .Should().BeEquivalentTo(new[] { "todo", "inprogress", "done" }, options => options.WithStrictOrdering());
        }
    }

    [Fact]
    public void TaskStateName_ShouldBeCaseInsensitive()
    {
        var name1 = new TaskStateName("todo");
        var name2 = new TaskStateName("TODO");

        name1.Should().Be(name2);
    }

    [Fact]
    public void TaskState_CanTransitionTo_ReturnsWhetherStateCanTransitionToGivenState()
    {
        var availableState = Guid.NewGuid();
        var unavailableState = Guid.NewGuid();

        var taskState = TaskState.Create(Guid.NewGuid(), new("test"), new[] { availableState }, 0);

        using(new AssertionScope())
        {
            taskState.CanTransitionTo(availableState).Should().BeTrue();
            taskState.CanTransitionTo(unavailableState).Should().BeFalse();
        }
    }
}
