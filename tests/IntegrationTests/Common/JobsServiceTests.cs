using Application.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;

namespace IntegrationTests.Common;

[Collection(nameof(IntegrationTestsCollection))]
public class JobsServiceTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public JobsServiceTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(_fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task RemoveUserFromOrganizationProjects_ShouldRemoveUserAsProjectMembersFromOrganizationProjects()
    {
        var users = await _factory.CreateUsers(2);
        var org = Organization.Create("org", users[0].Id);
        var invitation = org.CreateInvitation(users[1].Id).Value;
        invitation.Accept();
        var projects = new List<Project>();
        for(int i = 0; i < 2; ++i)
        {
            var project = Project.Create("proj" + i.ToString(), org.Id, users[0].Id);
            _ = project.AddMember(users[1].Id);
        }

        await _fixture.SeedDb(db =>
        {
            db.Add(org);
            db.AddRange(projects);
        });

        var projectMembersCountBefore = await _fixture.CountAsync<ProjectMember>();

        await _fixture.ExecuteOnService<IJobsService>(x => x.RemoveUserFromOrganizationProjects(users[1].Id, org.Id));

        (await _fixture.CountAsync<ProjectMember>()).Should().Be(projectMembersCountBefore - projects.Count);
    }
}
