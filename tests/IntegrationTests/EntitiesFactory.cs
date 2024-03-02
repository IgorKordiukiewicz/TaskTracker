using Infrastructure.Models;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.Workflows;
using Task = Domain.Tasks.Task;
using Domain.Tasks;

namespace IntegrationTests;

public class EntitiesFactory
{
    private readonly IntegrationTestsFixture _fixture;

    public EntitiesFactory(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task<List<User>> CreateUsers(int count = 1)
    {
        var users = CreateEntities(count, i => User.Create($"authId{DateTime.Now.Ticks}{i}", $"user{i}", "firstName", "lastName"));

        var usersPresentationData = users.Select(x => new UserPresentationData()
        {
            UserId = x.Id,
            AvatarColor = "#000000"
        });

        await _fixture.SeedDb(db =>
        {
            db.AddRange(users);  
            db.AddRange(usersPresentationData);
        });

        return users;
    }

    public async Task<List<Organization>> CreateOrganizations(int count = 1)
    {
        var user = (await CreateUsers())[0];

        var organizations = CreateEntities(count, i => Organization.Create($"org{i}", user.Id));

        await _fixture.SeedDb(db =>
        {
            db.AddRange(organizations);
        });

        return organizations;
    }

    public async Task<List<Project>> CreateProjects(int count = 1)
    {
        var organization = (await CreateOrganizations())[0];

        var projects = CreateEntities(count, i => Project.Create($"project{i}", organization.Id, organization.OwnerId));

        await _fixture.SeedDb(db =>
        {
            db.AddRange(projects);
        });

        return projects;
    }

    public async Task<List<Workflow>> CreateWorkflows(int count = 1)
    {
        var projects = await CreateProjects(count);

        var workflows = CreateEntities(count, i => Workflow.Create(projects[i].Id));

        await _fixture.SeedDb(db =>
        {
            db.AddRange(workflows);
        });

        return workflows;
    }

    public async Task<List<TaskRelationshipManager>> CreateTaskRelationshipManagers(int count = 1)
    {
        var workflows = await CreateWorkflows(count);

        var relationshipManagers = CreateEntities(count, i => new TaskRelationshipManager(workflows[i].ProjectId));

        await _fixture.SeedDb(db =>
        {
            db.AddRange(relationshipManagers);
        });

        return relationshipManagers;
    }

    public async Task<List<Task>> CreateTasks(int count = 1)
    {
        var relationshipManager = (await CreateTaskRelationshipManagers())[0];

        var initialStatus = await _fixture.FirstAsync<Domain.Workflows.TaskStatus>(x => x.Initial);
        var tasks = CreateEntities(count, i => Task.Create(1, relationshipManager.ProjectId, $"title{i}", $"desc{i}", initialStatus.Id));

        await _fixture.SeedDb(db =>
        {
            db.AddRange(tasks);
        });

        return tasks;
    }

    private static List<TEntity> CreateEntities<TEntity>(int count, Func<int, TEntity> selector) where TEntity : class
        => Enumerable.Range(0, count).Select(selector).ToList();
}
