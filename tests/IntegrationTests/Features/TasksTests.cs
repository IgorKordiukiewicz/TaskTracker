using Application.Features.Tasks;
using Domain.Organizations;
using Domain.Projects;
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
    public async System.Threading.Tasks.Task Create_ShouldCreateTask_WithCorrectShortId_WhenProjectExists()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        var task1 = Task.Create(1, project.Id, "title1", "desc1");
        var task2 = Task.Create(2, project.Id, "title2", "desc2");
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
            await db.Tasks.AddRangeAsync(new[] { task1, task2 });
        });

        var result = await _fixture.SendRequest(new CreateTaskCommand(project.Id, _autoFixture.Create<CreateTaskDto>()));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeEmpty();
            var task = await _fixture.FirstAsync<Task>(x => x.Id == result.Value);
            task.ShortId.Should().Be(3);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldReturnProjectTasks()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project1 = Project.Create("project", organization.Id, user.Id);
        var project2 = Project.Create("project2", organization.Id, user.Id);
        var task1 = Task.Create(1, project1.Id, "title1", "desc1");
        var task2 = Task.Create(2, project1.Id, "title2", "desc2");
        var task3 = Task.Create(1, project2.Id, "title3", "desc3");
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddRangeAsync(new[] { project1, project2 });
            await db.Tasks.AddRangeAsync(new[] { task1, task2, task3 });
        });

        var result = await _fixture.SendRequest(new GetAllTasksQuery(project1.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Tasks.Should().HaveCount(2);
        }
    }
}
