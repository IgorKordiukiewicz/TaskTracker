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
    public async System.Threading.Tasks.Task Create_ShouldCreateTask_WithCorrectShortIdAndInitialTaskState_WhenProjectExists()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var taskStatesManager = TaskStatesManager.Create(project.Id);
        var initialStateId = taskStatesManager.AllStates.First(x => x.IsInitial).Id;
        var task1 = Task.Create(1, project.Id, "title1", "desc1", initialStateId);
        var task2 = Task.Create(2, project.Id, "title2", "desc2", initialStateId);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.TaskStatesManagers.AddAsync(taskStatesManager);
            await db.Tasks.AddRangeAsync(new[] { task1, task2 });
        });

        var result = await _fixture.SendRequest(new CreateTaskCommand(project.Id, _autoFixture.Create<CreateTaskDto>()));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeEmpty();
            var task = await _fixture.FirstAsync<Task>(x => x.Id == result.Value);
            task.ShortId.Should().Be(3);

            var initialTaskState = await _fixture.FirstAsync<TaskState>(x => x.IsInitial);
            task.StateId.Should().Be(initialTaskState.Id);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldFail_WhenTaskStatesManagerDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetAllTasksQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldReturnProjectTasksAndAllPossibleStates()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project1 = Project.Create("project", organization.Id, user.Id);
        var project2 = Project.Create("project2", organization.Id, user.Id);
        var taskStatesManager1 = TaskStatesManager.Create(project1.Id);
        var initialStateId1 = taskStatesManager1.AllStates.First(x => x.IsInitial).Id;
        var taskStatesManager2 = TaskStatesManager.Create(project2.Id);
        var initialStateId2 = taskStatesManager1.AllStates.First(x => x.IsInitial).Id;
        var task1 = Task.Create(1, project1.Id, "title1", "desc1", initialStateId1);
        var task2 = Task.Create(2, project1.Id, "title2", "desc2", initialStateId1);
        var task3 = Task.Create(1, project2.Id, "title3", "desc3", initialStateId2);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddRangeAsync(new[] { project1, project2 });
            await db.TaskStatesManagers.AddRangeAsync(new[] { taskStatesManager1, taskStatesManager2 });
            await db.Tasks.AddRangeAsync(new[] { task1, task2, task3 });
        });

        var result = await _fixture.SendRequest(new GetAllTasksQuery(project1.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Tasks.Should().HaveCount(2);
            result.Value.AllTaskStates.Should().HaveCount(taskStatesManager1.AllStates.Count);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateState_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskStateCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateState_ShouldUpdateTaskState_WhenValid()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var taskStatesManager = TaskStatesManager.Create(project.Id);
        var initialState = taskStatesManager.AllStates.First(x => x.IsInitial);
        var task = Task.Create(1, project.Id, "title", "desc", initialState.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.TaskStatesManagers.AddAsync(taskStatesManager);
            await db.Tasks.AddAsync(task);
        });

        var newStateId = initialState.PossibleNextStates.First();

        var result = await _fixture.SendRequest(new UpdateTaskStateCommand(task.Id, newStateId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var updatedTask = await _fixture.FirstAsync<Task>(x => x.Id == task.Id);
            updatedTask.StateId.Should().Be(newStateId);
        }
    }
}
