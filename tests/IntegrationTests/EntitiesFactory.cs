using Infrastructure.Models;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.Workflows;
using Task = Domain.Tasks.Task;
using Domain.Tasks;
using Application.Models.ViewModels;

namespace IntegrationTests;

public class EntitiesFactory(IntegrationTestsFixture fixture)
{
    public async Task<List<User>> CreateUsers(int count = 1)
    {
        var users = CreateEntities(count, i => User.Create(Guid.NewGuid(), $"user{i}", "firstName", "lastName"));

        var usersPresentationData = users.Select(x => new UserPresentationData()
        {
            UserId = x.Id,
            AvatarColor = "#000000"
        });

        await fixture.SeedDb(db =>
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

        await fixture.SeedDb(db =>
        {
            db.AddRange(organizations);
        });

        return organizations;
    }

    public async Task<List<Project>> CreateProjects(int count = 1)
    {
        var organization = (await CreateOrganizations())[0];

        var projects = CreateEntities(count, i => Project.Create($"project{i}", organization.Id, organization.OwnerId));

        await fixture.SeedDb(db =>
        {
            db.AddRange(projects);
        });

        return projects;
    }

    public async Task<List<Workflow>> CreateWorkflows(int count = 1)
    {
        var projects = await CreateProjects(count);

        var workflows = CreateEntities(count, i => Workflow.Create(projects[i].Id));

        await fixture.SeedDb(db =>
        {
            db.AddRange(workflows);
        });

        return workflows;
    }

    public async Task<List<TaskRelationshipManager>> CreateTaskRelationshipManagers(int count = 1)
    {
        var workflows = await CreateWorkflows(count);

        var relationshipManagers = CreateEntities(count, i => new TaskRelationshipManager(workflows[i].ProjectId));

        await fixture.SeedDb(db =>
        {
            db.AddRange(relationshipManagers);
        });

        return relationshipManagers;
    }

    public async Task<List<Task>> CreateTasks(int count = 1)
    {
        var relationshipManager = (await CreateTaskRelationshipManagers())[0];

        var statuses = await fixture.GetAsync<Domain.Workflows.TaskStatus>();

        var initialStatus = await fixture.FirstAsync<Domain.Workflows.TaskStatus>(x => x.Initial);
        var tasks = CreateEntities(count, i => Task.Create(1, relationshipManager.ProjectId, DateTime.Now, $"title{i}", $"desc{i}", initialStatus.Id));

        var tasksBoardLayout = new TasksBoardLayout()
        {
            ProjectId = relationshipManager.ProjectId,
            Columns = statuses.Select(x => new TasksBoardColumn()
            {
                StatusId = x.Id,
                TasksIds = x.Id == initialStatus.Id ? tasks.Select(xx => xx.Id).ToList() : []
            }).ToArray()
        };

        await fixture.SeedDb(db =>
        {
            db.AddRange(tasks);
            db.Add(tasksBoardLayout);
        });

        return tasks;
    }

    private static List<TEntity> CreateEntities<TEntity>(int count, Func<int, TEntity> selector) where TEntity : class
        => Enumerable.Range(0, count).Select(selector).ToList();
}
