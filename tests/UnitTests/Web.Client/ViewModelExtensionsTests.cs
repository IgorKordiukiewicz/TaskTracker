using Shared.ViewModels;
using Web.Client.Common;

namespace UnitTests.Web.Client;

public class ViewModelExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void GetAvailableFromStatuses_ShouldReturnEmptyList_WhenWorkflowVMIsNull()
    {
        var result = ((WorkflowVM?)null).GetAvailableFromStatuses();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAvailableFromStatuses_ShouldReturnStatuses_ThatAreNotTransitiableToEveryStatus()
    {
        var statusesIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var transitions = new List<WorkflowTaskStatusTransitionVM>()
        {
            new(statusesIds[0], statusesIds[1]),
            new(statusesIds[0], statusesIds[2]),
            new(statusesIds[0], statusesIds[3]),
            new(statusesIds[1], statusesIds[0]),
            new(statusesIds[2], statusesIds[3]),
        };

        var workflowVM = CreateWorkflowVM(statusesIds, transitions);
        var statuses = workflowVM.Statuses;

        var result = workflowVM.GetAvailableFromStatuses();

        result.Should().BeEquivalentTo(new[]
        {
            statuses[1].Name, statuses[2].Name, statuses[3].Name
        });
    }

    [Fact]
    public void GetAvailableToStatuses_ShouldReturnEmptyList_WhenWorkflowVMIsNull()
    {
        var result = ((WorkflowVM?)null).GetAvailableToStatuses("abc", new Dictionary<string, Guid>());

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAvailableToStatuses_ShouldReturnEmptyList_WhenStatusIdByNameIsNull()
    {
        var workflowVM = _fixture.Create<WorkflowVM>();
        var result = workflowVM.GetAvailableToStatuses("abc", null);

        result.Should().BeEmpty();
    }

    [Fact] 
    public void GetAvailableToStatuses_ShouldReturnStatuses_ThatDontAlreadyHaveTransitionFromGivenStatus()
    {
        var statusesIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var transitions = new List<WorkflowTaskStatusTransitionVM>()
        {
            new(statusesIds[0], statusesIds[1]),
            new(statusesIds[1], statusesIds[0]),
            new(statusesIds[0], statusesIds[2]),
        };

        var workflowVM = CreateWorkflowVM(statusesIds, transitions);
        var statuses = workflowVM.Statuses;
        var statusNameComparer = StringComparer.OrdinalIgnoreCase;
        var statusIdByName = workflowVM.Statuses.ToDictionary(k => k.Name, v => v.Id, statusNameComparer);

        var result = workflowVM.GetAvailableToStatuses(statuses[0].Name, statusIdByName);

        result.Should().BeEquivalentTo(new[] { statuses[3].Name });
    }

    private WorkflowVM CreateWorkflowVM(Guid[] statusesIds, List<WorkflowTaskStatusTransitionVM> transitions)
    {
        var statuses = _fixture.CreateMany<WorkflowTaskStatusVM>(4).ToList();
        for (int i = 0; i < statusesIds.Length; ++i)
        {
            statuses[i] = statuses[i] with { Id = statusesIds[i] };
        }

        return _fixture.Build<WorkflowVM>()
            .With(x => x.Statuses, statuses)
            .With(x => x.Transitions, transitions)
            .Create();
    }
}
