using Domain.Tasks;
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
        var stateId = Guid.NewGuid();

        var result = Task.Create(shortId, projectId, title, description, stateId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.ShortId.Should().Be(shortId);
            result.ProjectId.Should().Be(projectId);
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.StateId.Should().Be(stateId);
        }
    }

    [Fact]
    public void UpdateState_ShouldFail_WhenNewStateIdIsNotValid()
    {
        var statesManager = new TaskStatesManager(Guid.NewGuid());
        var initialState = statesManager.AllStates.First(x => x.IsInitial);
        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialState.Id);

        var result = task.UpdateState(Guid.NewGuid(), statesManager);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateState_ShouldFail_WhenStateCannotTransitionToNewState()
    {
        var statesManager = new TaskStatesManager(Guid.NewGuid());
        var initialState = statesManager.AllStates.First(x => x.IsInitial);
        var unavailableState = statesManager.AllStates.First(x => !initialState.CanTransitionTo(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialState.Id);

        var result = task.UpdateState(unavailableState.Id, statesManager);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateState_ShouldUpdateStateId_WhenStateCanTransitionToNewState()
    {
        var statesManager = new TaskStatesManager(Guid.NewGuid());
        var initialState = statesManager.AllStates.First(x => x.IsInitial);
        var availableState = statesManager.AllStates.First(x => initialState.CanTransitionTo(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialState.Id);

        var result = task.UpdateState(availableState.Id, statesManager);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            task.StateId.Should().Be(availableState.Id);
        }
    }
}
