using Infrastructure.Models;
using Application.Features.Users;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Shared.Enums;
using Shared.ViewModels;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class UsersTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

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
        // TODO: Check if permissions are correctly returned
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new GetUserQuery(user.AuthenticationId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PermissionsByOrganization = new Dictionary<Guid, OrganizationPermissions>(),
                PermissionsByProject = new Dictionary<Guid, ProjectPermissions>()
            });
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

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(user.AuthenticationId, "email", "firstName", "lastName", "#000000")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterUser_ShouldAddNewUserAndUserPresentationData_WhenUserIsNotRegistered()
    {
        var newUserAuthId = "999";
        await _factory.CreateUsers();
        var avatarColor = "#000000";

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(newUserAuthId, "email", "firstName", "lastName", avatarColor)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var users = await _fixture.GetAsync<User>();
            users.Count.Should().Be(2);
            var newUser = users.First(x => x.AuthenticationId == newUserAuthId);
            newUser.Should().NotBeNull();
            (await _fixture.FirstAsync<UserPresentationData>(x => x.UserId == newUser.Id)).AvatarColor.Should().Be(avatarColor);
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

    [Fact]
    public async Task UpdateUserName_ShouldFail_WhenUserWithGivenAuthIdDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateUserNameCommand(Guid.NewGuid(), new("first", "last")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUserName_ShouldUpdateFirstAndLastName_WhenUserWithGivenAuthIdExists()
    {
        var user = (await _factory.CreateUsers())[0];
        var newFirstName = user.FirstName + "A";
        var newLastName = user.LastName + "A";

        var result = await _fixture.SendRequest(new UpdateUserNameCommand(user.Id, new(newFirstName, newLastName)));

        using(new AssertionScope()) 
        {
            result.IsSuccess.Should().BeTrue();
            var updatedUser = await _fixture.FirstAsync<User>(x => x.Id == user.Id);
            updatedUser.FirstName.Should().Be(newFirstName);
            updatedUser.LastName.Should().Be(newLastName);
        }
    }

    [Fact]
    public async Task GetAllUsersPresentationData_ShouldFail_WhenUserDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetAllUsersPresentationDataQuery("_"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllUsersPresentationData_ShouldReturnPresentationDataForAllUsersTheUserCanSee_WhenUserExists()
    {
        var users =  await _factory.CreateUsers(3);
        var organization = Organization.Create("org", users[0].Id);
        var invitation = organization.CreateInvitation(users[1].Id).Value;
        _ = organization.AcceptInvitation(invitation.Id);

        await _fixture.SeedDb(db =>
        {
            db.Organizations.Add(organization);
        });

        var result = await _fixture.SendRequest(new GetAllUsersPresentationDataQuery(users[0].AuthenticationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Data.Select(x => x.UserId).Should().BeEquivalentTo(new[]
            {
                users[0].Id, users[1].Id
            });
        }
    }
}
