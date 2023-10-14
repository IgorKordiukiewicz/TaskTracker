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

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void AddTransition_ShouldFail_WhenFromOrToStatusDoesNotExist(bool existingFromStatus, bool existingToStatus)
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var status1 = workflow.Statuses[0];
        var status2 = workflow.Statuses[1];

        var result = workflow.AddTransition(existingFromStatus ? status1.Id : Guid.NewGuid(), existingToStatus ? status2.Id : Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddTransition_ShouldSucceed_WhenBothStatusesExistAndTransitionDoesNotExist()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        _ = workflow.AddStatus("test");
        var fromStatus = workflow.Statuses.First(x => x.Name != "test");
        var toStatus = workflow.Statuses.First(x => x.Name == "test");

        var result = workflow.AddTransition(fromStatus.Id, toStatus.Id);

        result.IsSuccess.Should().BeTrue();
    }
}
