using Application.Features.Tasks;
using Application.Models.Dtos;
using Application.Models.ViewModels;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Models;
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
        _ = await _factory.CreateTasks(2);

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

            var initialTaskStatus = await _fixture.FirstAsync<TaskStatus>(x => x.Initial);
            task.StatusId.Should().Be(initialTaskStatus.Id);
        }
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 2)]
    public async System.Threading.Tasks.Task Get_ShouldReturnProjectTasksAndAllPossibleStatuses(bool useList, int expectedCount)
    {
        var workflows = await _factory.CreateWorkflows(2);

        var taskBoardLayouts = workflows.Select(x => new TasksBoardLayout()
        {
            ProjectId = x.ProjectId,
            Columns = x.Statuses.Select(xx => new TasksBoardColumn()
            {
                StatusId = xx.Id,
                TasksIds = []
            }).ToArray()
        }).ToList();

        var initialStatusId1 = workflows[0].Statuses.First(x => x.Initial).Id;
        var initialStatusId2 = workflows[1].Statuses.First(x => x.Initial).Id;
        var task1 = Task.Create(1, workflows[0].ProjectId, DateTime.Now, "title1", "desc1", initialStatusId1);
        var task2 = Task.Create(2, workflows[0].ProjectId, DateTime.Now, "title2", "desc2", initialStatusId1);
        var task3 = Task.Create(1, workflows[1].ProjectId, DateTime.Now, "title3", "desc3", initialStatusId2);

        taskBoardLayouts[0].Columns.First(x => x.StatusId == initialStatusId1).TasksIds.AddRange([task1.Id, task2.Id]);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(task1, task2, task3);
            db.AddRange(taskBoardLayouts);
        });

        var result = await _fixture.SendRequest(new GetTasksQuery(workflows[0].ProjectId, useList ? new[] { task1.Id, task2.Id } : task1.ShortId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Tasks.Should().HaveCount(expectedCount);
            result.Value.AllTaskStatuses.Should().HaveCount(workflows[0].Statuses.Count);
            result.Value.BoardColumns.Should().HaveCount(workflows[0].Statuses.Count);
            result.Value.BoardColumns.SelectMany(x => x.TasksIds).Should().HaveCount(2);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldFail_WhenTaskNewStatusIdDoesNotExist()
    {
        var task = (await _factory.CreateTasks())[0];

        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(task.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatus_ShouldUpdateTaskStatus_WhenValid()
    {
        var task = (await _factory.CreateTasks())[0];

        var initialStatus = await _fixture.FirstAsync<TaskStatus>(x => x.Initial);
        var newStatusId = (await _fixture.FirstAsync<TaskStatusTransition>(x => x.FromStatusId == initialStatus.Id)).ToStatusId;

        var result = await _fixture.SendRequest(new UpdateTaskStatusCommand(task.Id, new(newStatusId)));

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
        var result = await _fixture.SendRequest(new AddTaskCommentCommand(Guid.NewGuid(), Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddComment_ShouldAddComment_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var user = await _fixture.FirstAsync<User>();
        var taskCommentsCountBefore = await _fixture.CountAsync<TaskComment>();

        var result = await _fixture.SendRequest(new AddTaskCommentCommand(task.Id, user.Id, new("abc")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskComment>()).Should().Be(taskCommentsCountBefore + 1);
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

    [Fact]
    public async System.Threading.Tasks.Task UpdateTitle_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskTitleCommand(Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTitle_ShouldSucceed_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var newTitle = task.Title + "A";

        var result = await _fixture.SendRequest(new UpdateTaskTitleCommand(task.Id, new(newTitle)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Task>(x => x.Id == task.Id)).Title.Should().Be(newTitle);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateDescription_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskDescriptionCommand(Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateDescription_ShouldSucceed_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var newDescription = "abab";

        var result = await _fixture.SendRequest(new UpdateTaskDescriptionCommand(task.Id, new(newDescription)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Task>(x => x.Id == task.Id)).Description.Should().Be(newDescription);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteTaskCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_ShouldSucceed_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];

        var result = await _fixture.SendRequest(new DeleteTaskCommand(task.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<Task>()).Should().Be(0);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetActivities_ShouldReturnActivities_WhenTaskExists()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var user = await _fixture.FirstAsync<User>();
        var task = Task.Create(1, workflow.ProjectId, DateTime.Now, "title", "desc", workflow.Statuses.First(x => x.Initial).Id);

        var transition = workflow.Transitions.First(x => x.FromStatusId == task.StatusId);
        var oldStatus = workflow.Statuses.First(x => x.Id == task.StatusId);
        var newStatus = workflow.Statuses.First(x => x.Id == transition.ToStatusId);

        task.UpdateAssignee(user.Id, DateTime.Now);
        task.Unassign(DateTime.Now);
        task.UpdateStatus(newStatus.Id, workflow, DateTime.Now);

        await _fixture.SeedDb(db =>
        {
            db.Tasks.Add(task);
        });

        var result = await _fixture.SendRequest(new GetTaskActivitiesQuery(task.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var activities = result.Value.Activities;
            activities.Should().HaveCount(4);
            AssertActivities(activities[0], TaskProperty.Status, oldStatus.Name, newStatus.Name);
            AssertActivities(activities[1], TaskProperty.Assignee, user.FullName, null);
            AssertActivities(activities[2], TaskProperty.Assignee, null, user.FullName);
            AssertActivities(activities[3], TaskProperty.Creation, null, null);
            
            static void AssertActivities(TaskActivityVM activity, TaskProperty expectedProperty, string? expectedOldValue, string? expectedNewValue)
            {
                activity.Property.Should().Be(expectedProperty);
                activity.OldValue.Should().Be(expectedOldValue);
                activity.NewValue.Should().Be(expectedNewValue);
            }
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task LogTime_ShouldFail_WhenTaskDoesNotExist()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new LogTaskTimeCommand(user.Id, Guid.NewGuid(), 
            new(1, DateTime.Now)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task LogTime_ShouldAddNewTimeLog_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var user = await _fixture.FirstAsync<User>();
        var timeLogsBefore = await _fixture.CountAsync<TaskTimeLog>();

        var result = await _fixture.SendRequest(new LogTaskTimeCommand(user.Id, task.Id, 
            new(1, DateTime.Now)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskTimeLog>()).Should().Be(timeLogsBefore + 1);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateEstimatedTime_ShouldFail_WhenTaskDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskEstimatedTimeCommand(Guid.NewGuid(), new(1)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateEstimatedTime_ShouldUpdateEstimatedTime_WhenTaskExists()
    {
        var task = (await _factory.CreateTasks())[0];
        const int newEstimatedTime = 10;

        var result = await _fixture.SendRequest(new UpdateTaskEstimatedTimeCommand(task.Id, new(newEstimatedTime)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Task>(x => x.Id == task.Id)).EstimatedTime.Should().Be(newEstimatedTime);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task AddHierarchicalTaskRelationship_ShouldFail_WhenRelationshipManagerDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddHierarchicalTaskRelationshipCommand(Guid.NewGuid(), new(Guid.NewGuid(), Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddHierarchicalTaskRelationship_ShouldAddNewHierarchicalRelationship_WhenRelationshipManagerExists()
    {
        var tasks = await _factory.CreateTasks(2);

        var result = await _fixture.SendRequest(new AddHierarchicalTaskRelationshipCommand(tasks[0].ProjectId, new(tasks[0].Id, tasks[1].Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var expectedRelationship = new TaskHierarchicalRelationship(tasks[0].Id, tasks[1].Id);
            var relationship = await _fixture.FirstAsync<TaskHierarchicalRelationship>();
            relationship.Should().Be(expectedRelationship);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveHierarchicalTaskRelationship_ShouldFail_WhenRelationshipManagerDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveHierarchicalTaskRelationshipCommand(Guid.NewGuid(), new(Guid.NewGuid(), Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveHierarchicalTaskRelationship_ShouldRemoveHierarchicalRelationship_WhenRelationshipManagerExists()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var relationshipManager = new TaskRelationshipManager(workflow.ProjectId);
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var tasks = new Task[]
        {
            Task.Create(1, relationshipManager.ProjectId, DateTime.Now, "a", "desc", initialStatus.Id),
            Task.Create(2, relationshipManager.ProjectId, DateTime.Now, "b", "desc", initialStatus.Id),
        };
        var tasksIds = tasks.Select(x => x.Id);
        _ = relationshipManager.AddHierarchicalRelationship(tasks[0].Id, tasks[1].Id, tasksIds);

        await _fixture.SeedDb(db =>
        {
            db.Add(relationshipManager);
            db.AddRange(tasks);
        });

        var result = await _fixture.SendRequest(new RemoveHierarchicalTaskRelationshipCommand(tasks[0].ProjectId, new(tasks[0].Id, tasks[1].Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<TaskHierarchicalRelationship>()).Should().Be(0);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskRelationships_ShouldReturnTaskRelationships_WhenTaskExists()
    {
        var workflow = (await _factory.CreateWorkflows())[0];
        var relationshipManager = new TaskRelationshipManager(workflow.ProjectId);
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var tasks = new Task[]
        {
            Task.Create(1, relationshipManager.ProjectId, DateTime.Now, "a", "desc", initialStatus.Id),
            Task.Create(2, relationshipManager.ProjectId, DateTime.Now, "b", "desc", initialStatus.Id),
            Task.Create(3, relationshipManager.ProjectId, DateTime.Now, "c", "desc", initialStatus.Id),
        };
        var tasksIds = tasks.Select(x => x.Id);
        _ = relationshipManager.AddHierarchicalRelationship(tasks[0].Id, tasks[1].Id, tasksIds);
        _ = relationshipManager.AddHierarchicalRelationship(tasks[1].Id, tasks[2].Id, tasksIds);
        // a -> b -> c
    
        await _fixture.SeedDb(db =>
        {
            db.Add(relationshipManager);
            db.AddRange(tasks);
        });
    
        var result = await _fixture.SendRequest(new GetTaskRelationshipsQuery(tasks[1].Id));
    
        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Parent.Should().NotBeNull();
            result.Value.Parent!.Id.Should().Be(tasks[0].Id);
    
            var childrenHierarchy = result.Value.ChildrenHierarchy;
            childrenHierarchy.Should().NotBeNull();
            childrenHierarchy!.TaskId.Should().Be(tasks[1].Id);
            childrenHierarchy.Children.Should().HaveCount(1);
            childrenHierarchy.Children[0].TaskId.Should().Be(tasks[2].Id);
            childrenHierarchy.Children[0].Children.Should().BeEmpty();
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskBoard_ShouldFail_WhenBoardLayoutDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateTaskBoardCommand(new(Guid.NewGuid(), [])));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskBoard_ShouldFail_WhenLayoutStatusesDontMatchExistingTasks()
    {
        var tasks = await _factory.CreateTasks();
        var workflow = await _fixture.FirstAsync<Workflow>();

        var columns = workflow.Statuses
            .Select(x => new UpdateTaskBoardColumnDto(x.Id, x.Id == tasks[0].StatusId ? [tasks[0].Id] : []))
            .ToList();
        columns.Add(new UpdateTaskBoardColumnDto(Guid.NewGuid(), []));
        var model = new UpdateTaskBoardDto(tasks[0].ProjectId, columns);

        var result = await _fixture.SendRequest(new UpdateTaskBoardCommand(model));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskBoard_ShouldFail_WhenLayoutTasksDontMatchExistingTasks()
    {
        var tasks = await _factory.CreateTasks();
        var workflow = await _fixture.FirstAsync<Workflow>();

        var columns = workflow.Statuses
            .Select(x => new UpdateTaskBoardColumnDto(x.Id, []))
            .ToList();
        var model = new UpdateTaskBoardDto(tasks[0].ProjectId, columns);

        var result = await _fixture.SendRequest(new UpdateTaskBoardCommand(model));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskBoard_ShouldSucceed_WhenModelValidationPassed()
    {
        var tasks = await _factory.CreateTasks(2);
        var statuses = await _fixture.GetAsync<TaskStatus>();

        var columns = statuses
            .Select(x => new UpdateTaskBoardColumnDto(x.Id, x.Id == tasks[0].StatusId ? [tasks[1].Id, tasks[0].Id] : []))
            .ToList();
        var model = new UpdateTaskBoardDto(tasks[0].ProjectId, columns);

        var result = await _fixture.SendRequest(new UpdateTaskBoardCommand(model));

        result.IsSuccess.Should().BeTrue();
    }
}
