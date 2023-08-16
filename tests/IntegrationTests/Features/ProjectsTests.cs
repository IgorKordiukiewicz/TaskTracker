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

    public ProjectsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;

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
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
        });

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, "123", new("project")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProject_WhenValidationPassed()
    {
        var userAuthId = "123";
        var user = User.Create(userAuthId, "user");
        var organization = Organization.Create("org", user.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
        });

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, userAuthId, new("project")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
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
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddRangeAsync(new[] { user1, user2 });
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddRangeAsync(new[] { project1, project2, project3 });
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
        var user = User.Create("123", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
        });

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

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddRangeAsync(new[] { user1, user2 });
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
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
    public async Task RemoveMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShuldRemoveMember_WhenProjectExists()
    {
        var user = User.Create("123", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id, user.Id);

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(project.Id, project.Members[0].Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
        }
    }
}
