using Application.Features.Workflows;
using Domain.Tasks;
using System.Threading.Tasks;
using Task = Domain.Tasks.Task;
using TaskStatus = Domain.Tasks.TaskStatus;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class WorkflowsTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public WorkflowsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetForProject_ShouldFail_WhenWorkflowForProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetWorkflowForProjectQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetForProject_ShouldReturnWorkflow_WhenWorkflowForProjectExists()
    {
        var workflow = (await _factory.CreateWorkflows())[0];

        var result = await _fixture.SendRequest(new GetWorkflowForProjectQuery(workflow.ProjectId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            result.Value.Id.Should().Be(workflow.Id);
            result.Value.Statuses.Count.Should().Be(workflow.Statuses.Count);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task AddTaskStatus_ShouldFail_WhenWorkflowDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddWorkflowTaskStatusCommand(Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddTaskStatus_ShouldCreateNewStatus_WhenWorkflowExists()
    {
        var workflow = (await _factory.CreateWorkflows())[0];

        var statusesBefore = await _fixture.CountAsync<TaskStatus>();

        var result = await _fixture.SendRequest(new AddWorkflowTaskStatusCommand(workflow.Id, new("abc")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskStatus>()).Should().Be(statusesBefore + 1);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task AddTransition_ShouldFail_WhenWorkflowDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddWorkflowTransitionCommand(Guid.NewGuid(), new(Guid.NewGuid(), Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddTransition_ShouldSucceed_WhenWorkflowExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var workflow = Workflow.Create(project.Id);
        _ = workflow.AddStatus("from1");
        _ = workflow.AddStatus("to1");
        await _fixture.SeedDb(db =>
        {
            db.Add(workflow);
        });

        var statusesIds = workflow.Statuses.Select(x => x.Id).ToHashSet();
        var fromStatus = workflow.Statuses.First(x => x.Name == "from1");
        var toStatus = workflow.Statuses.First(x => x.Name == "to1");
        var transitionsCountBefore = await _fixture.CountAsync<TaskStatusTransition>();

        var result = await _fixture.SendRequest(new AddWorkflowTransitionCommand(workflow.Id, new(fromStatus.Id, toStatus.Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var transitionsCount = await _fixture.CountAsync<TaskStatusTransition>();
            transitionsCount.Should().Be(transitionsCountBefore + 1);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task CanStatusBeDeleted_ShouldFail_WhenWorkflowDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CanWorkflowStatusBeDeletedQuery(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task CanStatusBeDeleted_ShouldFail_WhenStatusDoesNotExist()
    {
        var workflow = (await _factory.CreateWorkflows())[0];

        var result = await _fixture.SendRequest(new CanWorkflowStatusBeDeletedQuery(workflow.Id, Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task CanStatusBeDeleted_ShouldReturnFalse_WhenStatusIsTheInitialStatus()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var initialStatus = workflow.Statuses.First(x => x.Initial);

        var result = await _fixture.SendRequest(new CanWorkflowStatusBeDeletedQuery(workflow.Id, initialStatus.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task CanStatusBeDeleted_ShouldReturnFalse_WhenStatusIsAlreadyUsedByATask()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var notInitialStatus = workflow.Statuses.First(x => !x.Initial);
        var task = Task.Create(1, workflow.ProjectId, "title", "desc", notInitialStatus.Id);
        await _fixture.SeedDb(db =>
        {
            db.Add(task); 
        });

        var result = await _fixture.SendRequest(new CanWorkflowStatusBeDeletedQuery(workflow.Id, task.StatusId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task CanStatusBeDeleted_ShouldReturnTrue_WhenStatusIsNotInitialAndNotUsedByATask()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var notInitialStatus = workflow.Statuses.First(x => !x.Initial);

        var result = await _fixture.SendRequest(new CanWorkflowStatusBeDeletedQuery(workflow.Id, notInitialStatus.Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteStatus_ShouldFail_WhenWorkflowDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteWorkflowStatusCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteStatus_ShouldFail_WhenStatusIsUsedByATask()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var notInitialStatus = workflow.Statuses.First(x => !x.Initial);
        var task = Task.Create(1, workflow.ProjectId, "title", "desc", notInitialStatus.Id);
        await _fixture.SeedDb(db =>
        {
            db.Add(task);
        });

        var result = await _fixture.SendRequest(new DeleteWorkflowStatusCommand(workflow.Id, task.StatusId));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteStatus_ShouldRemoveStatusAndRelatedTransitions_WhenStatusCanBeDeleted()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var notInitialStatus = workflow.Statuses.First(x => !x.Initial);
        var statusesCountBefore = workflow.Statuses.Count;
        var transitionsCountBefore = workflow.Transitions.Count;
        var statusTransitions = workflow.Transitions.Count(x =>
            x.FromStatusId == notInitialStatus.Id || x.ToStatusId == notInitialStatus.Id);

        var result = await _fixture.SendRequest(new DeleteWorkflowStatusCommand(workflow.Id, notInitialStatus.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskStatus>()).Should().Be(statusesCountBefore - 1);
            (await _fixture.CountAsync<TaskStatusTransition>()).Should().Be(transitionsCountBefore - statusTransitions);
        }
    }
}
