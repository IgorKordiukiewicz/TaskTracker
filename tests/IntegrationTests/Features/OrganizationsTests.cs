using Application.Features.Organizations;
using Domain.Organizations;
using Domain.Users;
using Shared.ViewModels;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class OrganizationsTests
{
    private readonly IntegrationTestsFixture _fixture;

    public OrganizationsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDb();
    }

    [Fact]
    public async Task Create_ShouldFail_WhenOwnerDoesNotExist()
    {
        await _fixture.SeedDb(async db =>
        {
            await db.AddAsync(User.Create("123", "123"));
        });

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new("org", Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewOrganization_WhenOwnerExists()
    {
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(User.Create("123", "123"));
        });
        var userId = (await _fixture.FirstAsync<User>()).Id;

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new("org", userId)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var organizationId = result.Value;
            var organization = await _fixture.FirstAsync<Organization>(x => x.Id == organizationId);
            organization.Should().NotBeNull();
            organization.Name.Should().Be("org");
        }
    }

    [Fact]
    public async Task CreateInvitation_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateOrganizationInvitationCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateInvitation_ShouldFail_WhenOUserDoesNotExist()
    {
        await _fixture.SeedDb(async db =>
        {
            var user = User.Create("123", "user");
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(Organization.Create("org", user.Id));
        });
        var organizationId = (await _fixture.FirstAsync<Organization>()).Id;

        var result = await _fixture.SendRequest(new CreateOrganizationInvitationCommand(organizationId, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateInvitation_ShouldCreateNewInvitation_WhenOrganizationAndUserBothExist()
    {
        var user = User.Create("123", "user");
        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user);
            await db.Organizations.AddAsync(Organization.Create("org", user.Id));
        });
        var organizationId = (await _fixture.FirstAsync<Organization>()).Id;

        var result = await _fixture.SendRequest(new CreateOrganizationInvitationCommand(organizationId, new(user.Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationInvitation>()).Should().Be(1);
        }
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenOrganizationWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AcceptOrganizationInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldAddNewOrganizationMemberAndUpdateInvitationState()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user1);
            await db.Users.AddAsync(user2);
            await db.Organizations.AddAsync(organization);
        });

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();

        var result = await _fixture.SendRequest(new AcceptOrganizationInvitationCommand(invitation.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationMember>()).Should().Be(membersBefore + 1);

            var orgMember = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitation.Id);
            orgMember.State.Should().Be(OrganizationInvitationState.Accepted);
        }
    }

    [Fact]
    public async Task DeclineInvitation_ShouldFail_WhenOrganizationWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeclineOrganizationInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeclineInvitation_ShouldUpdateInvitationState()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddAsync(user1);
            await db.Users.AddAsync(user2);
            await db.Organizations.AddAsync(organization);
        });

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();

        var result = await _fixture.SendRequest(new DeclineOrganizationInvitationCommand(invitation.Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var orgMember = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitation.Id);
            orgMember.State.Should().Be(OrganizationInvitationState.Declined);
        }
    }

    [Fact]
    public async Task GetForUser_ShouldReturnOrganizationsUserIsAMemberOf()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var org1 = Organization.Create("org1", user1.Id);
        var org2 = Organization.Create("org2", user2.Id);
        var org3 = Organization.Create("org3", user1.Id);

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddRangeAsync(new[] { user1, user2 });
            await db.Organizations.AddRangeAsync(new[] { org1, org2, org3 });
        });

        var result = await _fixture.SendRequest(new GetOrganizationsForUserQuery("123"));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var orgs = result.Value.Organizations;
            orgs.Should().BeEquivalentTo(new[]
            {
                new OrganizationForUserVM()
                {
                    Id = org1.Id,
                    Name = org1.Name,
                },
                new OrganizationForUserVM()
                {
                    Id = org3.Id,
                    Name = org3.Name
                },
            });
        }
    }

    [Fact]
    public async Task GetInvitationsForUser_ShouldFail_WhenUserDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetOrganizationInvitationsForUserQuery("111"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetInvitationsForUser_ShouldReturnUsersInvitations_WhenUserExists()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var org1 = Organization.Create("org1", user1.Id);
        var org2 = Organization.Create("org2", user2.Id);
        var org3 = Organization.Create("org3", user1.Id);
        var invitation1 = org1.CreateInvitation(user2.Id).Value;
        var invitation2 = org3.CreateInvitation(user2.Id).Value;

        await _fixture.SeedDb(async db =>
        {
            await db.Users.AddRangeAsync(new[] { user1, user2 });
            await db.Organizations.AddRangeAsync(new[] { org1, org2, org3 });
        });

        var result = await _fixture.SendRequest(new GetOrganizationInvitationsForUserQuery("1234"));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var invitations = result.Value.Invitations;
            invitations.Should().BeEquivalentTo(new[]
            {
                new OrganizationInvitationVM(invitation1.Id, org1.Name),
                new OrganizationInvitationVM(invitation2.Id, org3.Name),
            });
        }
    }
}
