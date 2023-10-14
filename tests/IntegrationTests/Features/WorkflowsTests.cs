using Application.Features.Workflows;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Task = Domain.Tasks.Task;
using TaskStatus = Domain.Tasks.TaskStatus;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class WorkflowsTests
{
    private readonly IntegrationTestsFixture _fixture;

    public WorkflowsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;

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
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var workflow = Workflow.Create(project.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Workflows.AddAsync(workflow);
        });

        var result = await _fixture.SendRequest(new GetWorkflowForProjectQuery(project.Id));

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
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var workflow = Workflow.Create(project.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Workflows.AddAsync(workflow);
        });

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
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var workflow = Workflow.Create(project.Id);
        _ = workflow.AddStatus("from1");
        _ = workflow.AddStatus("to1");
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Workflows.AddAsync(workflow);
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
}
