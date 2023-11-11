using Application.Features.Users;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Shared.ViewModels;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class UsersTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;
    private readonly string _authId = "123123";

    public UsersTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task GetUser_ShouldFail_WhenUserWithGivenAuthIdDoesntExist()
    {
        await _factory.CreateUsers();

        var result = await _fixture.SendRequest(new GetUserQuery("-"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetUser_ShouldReturnUserData_WhenUserWithGivenAuthIdEixsts()
    {
        var user = (await _factory.CreateUsers())[0];
        var organizationsIds = (await _fixture.GetAsync<Organization>()).Select(x => x.Id).ToList();
        var projectsIds = (await _fixture.GetAsync<Project>()).Select(x => x.Id).ToList();

        var result = await _fixture.SendRequest(new GetUserQuery(user.AuthenticationId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new UserVM(user.Id, user.FullName, user.Email, organizationsIds, projectsIds));
        }
    }

    [Fact]
    public async Task IsUserRegistered_ShouldReturnTrue_WhenUserWithGivenAuthIdExists()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new IsUserRegisteredQuery(user.AuthenticationId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }
    }

    [Fact]
    public async Task IsUserRegistered_ShouldReturnFalse_WhenUserWithGivenAuthIdDoesntExist()
    {
        await _factory.CreateUsers();

        var result = await _fixture.SendRequest(new IsUserRegisteredQuery("987654"));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }
    }

    [Fact]
    public async Task RegisterUser_ShouldFail_WhenUserIsAlreadyRegistered()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(user.AuthenticationId, "email", "firstName", "lastName")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterUser_ShouldAddNewUser_WhenUserIsNotRegistered()
    {
        var newUserAuthId = "999";
        await _factory.CreateUsers();

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(newUserAuthId, "email", "firstName", "lastName")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var users = await _fixture.GetAsync<User>();
            users.Count.Should().Be(2);
            users.First(x => x.AuthenticationId == newUserAuthId).Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetUsersAvailableForOrganizationInvitation_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetUsersAvailableForOrganizationInvitationQuery(Guid.NewGuid(), "user"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsersAvailableForOrganizationInvitation_ShouldReturnUsersNotInOrganizationAndWithoutPendingInvitations_WhenNameMatchesSearchQuery()
    {
        var users = await _factory.CreateUsers(4);
        var org = Organization.Create("org", users[0].Id);
        _ = org.CreateInvitation(users[2].Id);
        var invitation = org.CreateInvitation(users[1].Id).Value;
        org.DeclineInvitation(invitation.Id);

        await _fixture.SeedDb(db =>
        {
            db.Add(org);
        });

        var result = await _fixture.SendRequest(new GetUsersAvailableForOrganizationInvitationQuery(org.Id, "1")); // available users are : user[1] and user[3], so search for user[1]

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var availableUsers = result.Value.Users;
            availableUsers.Count.Should().Be(1);
            availableUsers[0].Id.Should().Be(users[1].Id);
        }
    }

    [Fact]
    public async Task GetUsersAvailableForProject_ShouldFail_WhenProjectDoesNotExist()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new GetUsersAvailableForProjectQuery(project.OrganizationId, Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsersAvailableForProject_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new GetUsersAvailableForProjectQuery(Guid.NewGuid(), project.Id));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsersAvailableForProject_ShouldReturnUsersFromOrganizationButNotInProject()
    {
        var users = await _factory.CreateUsers(3);
        var org = Organization.Create("org", users[0].Id);
        var invitation = org.CreateInvitation(users[1].Id).Value;
        _ = org.AcceptInvitation(invitation.Id);
        var project = Project.Create("project", org.Id, users[0].Id);

        await _fixture.SeedDb(db =>
        {
            db.Add(org);
            db.Add(project);
        });

        var result = await _fixture.SendRequest(new GetUsersAvailableForProjectQuery(org.Id, project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Users.Should().BeEquivalentTo(new[] 
            {
                new UserSearchVM()
                {
                    Id = users[1].Id,
                    Email = "user1"
                }
            });
        }
    }
}
