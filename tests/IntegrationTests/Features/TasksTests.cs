using Application.Features.Tasks;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Shared.Dtos;
using Task = Domain.Tasks.Task;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class TasksTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly Fixture _autoFixture = new();

    public TasksTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDb();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateTaskCommand(Guid.NewGuid(), _autoFixture.Create<CreateTaskDto>()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_ShouldCreateTask_WithCorrectShortIdAndInitialTaskStatus_WhenProjectExists()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var workflow = Workflow.Create(project.Id);
        var initialStatusId = workflow.Statuses.First(x => x.IsInitial).Id;
        var task1 = Task.Create(1, project.Id, "title1", "desc1", initialStatusId);
        var task2 = Task.Create(2, project.Id, "title2", "desc2", initialStatusId);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Workflows.AddAsync(workflow);
            await db.Tasks.AddRangeAsync(new[] { task1, task2 });
        });

        var result = await _fixture.SendRequest(new CreateTaskCommand(project.Id, _autoFixture.Create<CreateTaskDto>()));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeEmpty();
            var task = await _fixture.FirstAsync<Task>(x => x.Id == result.Value);
            task.ShortId.Should().Be(3);

            var initialTaskStatus = await _fixture.FirstAsync<Domain.Tasks.TaskStatus>(x => x.IsInitial);
            task.StatusId.Should().Be(initialTaskStatus.Id);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldFail_WhenWorkflowDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetAllTasksQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldReturnProjectTasksAndAllPossibleStatuses()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project1 = Project.Create("project", organization.Id, user.Id);
        var project2 = Project.Create("project2", organization.Id, user.Id);
        var workflow1 = Workflow.Create(project1.Id);
        var initialStatusId1 = workflow1.Statuses.First(x => x.IsInitial).Id;
        var workflow2 = Workflow.Create(project2.Id);
        var initialStatusId2 = workflow1.Statuses.First(x => x.IsInitial).Id;
        var task1 = Task.Create(1, project1.Id, "title1", "desc1", initialStatusId1);
        var task2 = Task.Create(2, project1.Id, "title2", "desc2", initialStatusId1);
        var task3 = Task.Create(1, project2.Id, "title3", "desc3", initialStatusId2);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddRangeAsync(new[] { project1, project2 });
            await db.Workflows.AddRangeAsync(new[] { workflow1, workflow2 });
            await db.Tasks.AddRangeAsync(new[] { task1, task2, task3 });
        });

        var result = await _fixture.SendRequest(new GetAllTasksQuery(project1.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Tasks.Should().HaveCount(2);
            result.Value.AllTaskStatuses.Should().HaveCount(workflow1.Statuses.Count);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldUpdateTaskStatus_WhenValid()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var workflow = Workflow.Create(project.Id);
        var initialStatus = workflow.Statuses.First(x => x.IsInitial);
        var task = Task.Create(1, project.Id, "title", "desc", initialStatus.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Workflows.AddAsync(workflow);
            await db.Tasks.AddAsync(task);
        });

        var newStatusId = initialStatus.PossibleNextStatuses[0];

        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(task.Id, newStatusId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var updatedTask = await _fixture.FirstAsync<Task>(x => x.Id == task.Id);
            updatedTask.StatusId.Should().Be(newStatusId);
        }
    }
}
