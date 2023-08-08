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
        var result = await _fixture.SendRequest(new CreateProjectCommand(Guid.NewGuid(), new("project")));

        result.IsFailed.Should().BeTrue();
    }


    [Fact]
    public async Task Create_ShouldFail_WhenProjectWithSameNameAlreadyExistsWithinOrganization()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project = Project.Create("project", organization.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddAsync(project);
        });

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, new("project")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProject_WhenValidationPassed()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
        });

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, new("project")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Get_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectsQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Get_ShouldReturnAListOfProjectsBelongingToOrganization()
    {
        var user = User.Create("authId", "user");
        var organization = Organization.Create("org", user.Id);
        var project1 = Project.Create("project1", organization.Id);
        var project2 = Project.Create("project2", organization.Id);
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(organization);
            await db.Projects.AddRangeAsync(new[] { project1, project2 });
        });

        var result = await _fixture.SendRequest(new GetProjectsQuery(organization.Id));

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
}
