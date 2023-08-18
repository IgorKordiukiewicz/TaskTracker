using Application.Features.Users;
using Domain.Organizations;
using Domain.Users;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class UsersTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly string _authId = "123123";

    public UsersTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDb();
    }

    [Fact]
    public async Task IsUserRegistered_ShouldReturnTrue_WhenUserWithGivenAuthIdExists()
    {
        await SeedUsers();

        var result = await _fixture.SendRequest(new IsUserRegisteredQuery(_authId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }
    }

    [Fact]
    public async Task IsUserRegistered_ShouldReturnFalse_WhenUserWithGivenAuthIdDoesntExist()
    {
        await SeedUsers();

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
        await SeedUsers();

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(_authId, "Name")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterUser_ShouldAddNewUser_WhenUserIsNotRegistered()
    {
        var newUserAuthId = "999";
        await SeedUsers();

        var result = await _fixture.SendRequest(new RegisterUserCommand(new(newUserAuthId, "Name")));

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
        var user1 = User.Create("111", "user1");
        var user2 = User.Create("222", "user2");
        var user3 = User.Create("333", "user3");
        var user4 = User.Create("444", "abc");
        var org = Organization.Create("org", user1.Id);
        _ = org.CreateInvitation(user3.Id);
        var invitation = org.CreateInvitation(user2.Id).Value;
        org.DeclineInvitation(invitation.Id);

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddRangeAsync(new[] { user1, user2, user3, user4 });
            await db.Organizations.AddAsync(org);
        });

        var result = await _fixture.SendRequest(new GetUsersAvailableForOrganizationInvitationQuery(org.Id, "user"));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var users = result.Value.Users;
            users.Count.Should().Be(1);
            users[0].Id.Should().Be(user2.Id);
        }
    }

    private async Task SeedUsers()
    {
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(User.Create(_authId, "Name"));
        });
    }
}
