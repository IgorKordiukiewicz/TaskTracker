using Application.Features.Organizations;
using Domain.Organizations;
using Domain.Users;
using Shared.ViewModels;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class OrganizationsTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public OrganizationsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task Create_ShouldFail_WhenOwnerDoesNotExist()
    {
        await _factory.CreateUsers();

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new("org", Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewOrganization_WhenOwnerExists()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new("org", user.Id)));

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
    public async Task CreateInvitation_ShouldFail_WhenUserDoesNotExist()
    {
        var organization = (await _factory.CreateOrganizations())[0];

        var result = await _fixture.SendRequest(new CreateOrganizationInvitationCommand(organization.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateInvitation_ShouldCreateNewInvitation_WhenOrganizationAndUserBothExist()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var newUser = User.Create("1234", "newUser");
        await _fixture.SeedDb(db =>
        {
            db.Add(newUser);
        });

        var result = await _fixture.SendRequest(new CreateOrganizationInvitationCommand(organization.Id, new(newUser.Id)));

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
        var invitationId = await CreateOrganizationWithInvitation();

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();

        var result = await _fixture.SendRequest(new AcceptOrganizationInvitationCommand(invitationId));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationMember>()).Should().Be(membersBefore + 1);

            var orgInvitation = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitationId);
            orgInvitation.State.Should().Be(OrganizationInvitationState.Accepted);
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
        var invitationId = await CreateOrganizationWithInvitation();

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();

        var result = await _fixture.SendRequest(new DeclineOrganizationInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var orgMember = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitationId);
            orgMember.State.Should().Be(OrganizationInvitationState.Declined);
        }
    }

    [Fact]
    public async Task GetForUser_ShouldReturnOrganizationsUserIsAMemberOf()
    {
        var organizations = await _factory.CreateOrganizations(2);
        var user = await _fixture.FirstAsync<User>();
        _ = await _factory.CreateOrganizations(); // organizations for different user

        var result = await _fixture.SendRequest(new GetOrganizationsForUserQuery(user.AuthenticationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var orgs = result.Value.Organizations;
            orgs.Should().BeEquivalentTo(new[]
            {
                new OrganizationForUserVM()
                {
                    Id = organizations[0].Id,
                    Name = organizations[0].Name,
                },
                new OrganizationForUserVM()
                {
                    Id = organizations[1].Id,
                    Name = organizations[1].Name
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

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.AddRange(org1, org2, org3);
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

    [Fact]
    public async Task GetMembers_ShouldReturnOrganizationMembers()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();
        await _fixture.SeedDb(db =>
        {
            db.Add(User.Create("1234", "newUser"));
        });

        var result = await _fixture.SendRequest(new GetOrganizationMembersQuery(organization.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Members.Should().BeEquivalentTo(new[]
            {
                new OrganizationMemberVM(organization.Members[0].Id, user.Name)
            });
        }
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveOrganizationMemberCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShouldRemoveMember_WhenOrganizationExists()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("456", "user2");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;
        organization.AcceptInvitation(invitation.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
        });

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();

        var result = await _fixture.SendRequest(new RemoveOrganizationMemberCommand(organization.Id, 
            organization.Members.First(x => x.UserId == user2.Id).Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationMember>()).Should().Be(membersBefore - 1);
        }
    }

    private async Task<Guid> CreateOrganizationWithInvitation()
    {
        var user1 = User.Create("123", "user1");
        var user2 = User.Create("1234", "user2");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
        });

        return invitation.Id;
    }
}
