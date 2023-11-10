using Application.Features.Tasks;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Shared.Dtos;
using Shared.Enums;
using Shared.ViewModels;
using Task = Domain.Tasks.Task;
using TaskStatus = Domain.Workflows.TaskStatus;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class TasksTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly Fixture _autoFixture = new();
    private readonly EntitiesFactory _factory;

    public TasksTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateTaskCommand(Guid.NewGuid(), _autoFixture.Create<CreateTaskDto>()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_ShouldFail_WhenMemberDoesNotExistAndNewAssigneeIdIsNotNull()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new CreateTaskCommand(project.Id, new CreateTaskDto()
        {
            Title = "abc",
            Description = "abc",
            AssigneeMemberId = Guid.NewGuid()
        }));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_ShouldCreateTask_WithCorrectShortIdAndInitialTaskStatus_WhenProjectExists()
    {
        var tasks = await _factory.CreateTasks(2);

        var project = await _fixture.FirstAsync<Project>();
        var member = await _fixture.FirstAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new CreateTaskCommand(project.Id, new CreateTaskDto()
        {
            Title = "abc",
            Description = "abc",
            AssigneeMemberId = member.Id
        }));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeEmpty();
            var task = await _fixture.FirstAsync<Task>(x => x.Id == result.Value);
            task.ShortId.Should().Be(3);

            var initialTaskStatus = await _fixture.FirstAsync<Domain.Workflows.TaskStatus>(x => x.Initial);
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
        var workflows = await _factory.CreateWorkflows(2);

        var initialStatusId1 = workflows[0].Statuses.First(x => x.Initial).Id;
        var initialStatusId2 = workflows[1].Statuses.First(x => x.Initial).Id;
        var task1 = Task.Create(1, workflows[0].ProjectId, "title1", "desc1", initialStatusId1);
        var task2 = Task.Create(2, workflows[0].ProjectId, "title2", "desc2", initialStatusId1);
        var task3 = Task.Create(1, workflows[1].ProjectId, "title3", "desc3", initialStatusId2);
        await _fixture.SeedDb(db =>
        {
            db.AddRange(task1, task2, task3);
        });

        var result = await _fixture.SendRequest(new GetAllTasksQuery(workflows[0].ProjectId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Tasks.Should().HaveCount(2);
            result.Value.AllTaskStatuses.Should().HaveCount(workflows[0].Statuses.Count);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldFail_WhenTaskNewStatusIdDoesNotExist()
    {
        var task = (await _factory.CreateTasks())[0];

        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(task.Id, Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldUpdateTaskStatus_WhenValid()
    {
        var task = (await _factory.CreateTasks())[0];

        var initialStatus = await _fixture.FirstAsync<TaskStatus>(x => x.Initial);
        var newStatusId = (await _fixture.FirstAsync<TaskStatusTransition>(x => x.FromStatusId == initialStatus.Id)).ToStatusId;

        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(task.Id, newStatusId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var updatedTask = await _fixture.FirstAsync<Task>(x => x.Id == task.Id);
            updatedTask.StatusId.Should().Be(newStatusId);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task AddComment_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddTaskCommentCommand(Guid.NewGuid(), "123", new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddComment_ShouldAddComment_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var user = await _fixture.FirstAsync<User>();
        var taskCommentsCountBefore = await _fixture.CountAsync<TaskComment>();

        var result = await _fixture.SendRequest(new AddTaskCommentCommand(task.Id, user.AuthenticationId, new("abc")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskComment>()).Should().Be(taskCommentsCountBefore + 1);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetComments_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetTaskCommentsQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetComments_ShouldReturnTaskComments_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var user = await _fixture.FirstAsync<User>();
        var earlierDate = new DateTime(2023, 10, 20);
        var laterDate = earlierDate.AddDays(1);
        task.AddComment("abc", user.Id, laterDate);
        task.AddComment("xyz", user.Id, earlierDate);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(task.Comments);
        });

        var result = await _fixture.SendRequest(new GetTaskCommentsQuery(task.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var comments = result.Value.Comments;
            comments.Should().BeEquivalentTo(new[]
            {
                new TaskCommentVM("xyz", user.FullName, earlierDate),
                new TaskCommentVM("abc", user.FullName, laterDate),
            }, options => options.WithStrictOrdering());
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAssignee_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskAssigneeCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAssignee_ShouldFail_WhenMemberDoesNotExistAndNewAssigneeIdIsNotNull()
    {
        var task = (await _factory.CreateTasks())[0];

        var result = await _fixture.SendRequest(new UpdateTaskAssigneeCommand(task.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAssignee_ShouldSucceed_WhenTaskAndMemberExistAndNewAssigneeIdIsNotNull()
    {
        var task = (await _factory.CreateTasks())[0];
        var member = await _fixture.FirstAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new UpdateTaskAssigneeCommand(task.Id, new(member.Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var updatedTask = await _fixture.FirstAsync<Task>();
            updatedTask.AssigneeId.Should().Be(member.UserId);
        }   
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAssignee_ShouldSucceed_WhenTaskExistsAndNewAssigneeIdIsNull()
    {
        var task = (await _factory.CreateTasks())[0];

        var result = await _fixture.SendRequest(new UpdateTaskAssigneeCommand(task.Id, new(null)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var updatedTask = await _fixture.FirstAsync<Task>();
            updatedTask.AssigneeId.Should().BeNull();
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdatePriority_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskPriorityCommand(Guid.NewGuid(), new(TaskPriority.Normal)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdatePriority_ShouldSucceed_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var newPriority = TaskPriority.Urgent;

        var result = await _fixture.SendRequest(new UpdateTaskPriorityCommand(task.Id, new(newPriority)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Task>(x => x.Id == task.Id)).Priority.Should().Be(newPriority);
        }
    }
}
