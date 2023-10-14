using Application.Features.Projects;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Shared.ViewModels;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class ProjectsTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public ProjectsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task Create_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateProjectCommand(Guid.NewGuid(), "123", new("project")));

        result.IsFailed.Should().BeTrue();
    }


    [Fact]
    public async Task Create_ShouldFail_WhenProjectWithSameNameAlreadyExistsWithinOrganization()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new CreateProjectCommand(project.OrganizationId, "123", new(project.Name)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProjectAndWorkflow_WhenValidationPassed()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, user.AuthenticationId, new("project")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
            var workflow = await _fixture.FirstAsync<Domain.Tasks.Workflow>(x => x.ProjectId == result.Value);
            workflow.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetForOrganization_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectsForOrganizationQuery(Guid.NewGuid(), "123"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetForOrganization_ShouldReturnAListOfProjectsUserIsAMemberOfInGivenOrganization()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("456", "user2");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;
        _ = organization.AcceptInvitation(invitation.Id);
        var project1 = Project.Create("project1", organization.Id, user1.Id);
        var project2 = Project.Create("project2", organization.Id, user1.Id);
        var project3 = Project.Create("project3", organization.Id, user2.Id);
        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.AddRange(project1, project2, project3);
        });

        var result = await _fixture.SendRequest(new GetProjectsForOrganizationQuery(organization.Id, "123"));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Projects.Should().BeEquivalentTo(new[]
            {
                new ProjectVM
                {
                    Id = project1.Id,
                    Name = project1.Name
                },
                new ProjectVM
                {
                    Id = project2.Id,
                    Name = project2.Name
                }
            });
        }
    }

    [Fact]
    public async Task AddMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddProjectMemberCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddMember_ShouldFail_WhenUserIsNotAMemberOfProjectsOrganization()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new AddProjectMemberCommand(project.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddMember_ShouldAddNewMember()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var organization = Organization.Create("org", user1.Id);
        var project = Project.Create("project", organization.Id, user2.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new AddProjectMemberCommand(project.Id, new(user1.Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore + 1);
        }
    }

    [Fact]
    public async Task GetMembers_ShouldReturnProjectMembers()
    {
        var project = (await _factory.CreateProjects())[0];
        var user = await _fixture.FirstAsync<User>();

        var result = await _fixture.SendRequest(new GetProjectMembersQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Members.Should().BeEquivalentTo(new[]
            {
                new ProjectMemberVM(project.Members[0].Id, user.Name)
            });
        }
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShuldRemoveMember_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(project.Id, project.Members[0].Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
        }
    }

    [Fact]
    public async Task GetProjectOrganization_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectOrganizationQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetProjectOrganization_ShouldReturnProjectsOrganizationData_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new GetProjectOrganizationQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.OrganizationId.Should().Be(project.OrganizationId);
        }
    }
}
