using Application.Features.Workflows;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Task = Domain.Tasks.Task;

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
            result.Value.TaskStatuses.Count.Should().Be(workflow.Statuses.Count);
        }
    }
}
