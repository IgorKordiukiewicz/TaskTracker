using Domain.Organizations;

namespace UnitTests.Domain;

public class OrganizationTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Create_ShouldCreateOrganizationWithGivenParameters()
    {
        var name = "OrgName";
        var ownerId = Guid.NewGuid();

        var organization = Organization.Create(name, ownerId);

        using (new AssertionScope())
        {
            organization.Name.Should().Be(name);
            organization.OwnerId.Should().Be(ownerId);
            organization.Id.Should().NotBeEmpty();
        }
    }

    [Fact]
    public void Create_ShouldCreateOrganizationWithOwnerAsMember()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        using (new AssertionScope())
        {
            organization.Members.Count.Should().Be(1);
            var member = organization.Members[0];
            member.Id.Should().NotBeEmpty();
            member.UserId.Should().Be(ownerId);
        }
    }

    [Fact]
    public void CreateInvitation_ShouldCreateInvitation()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        var result = organization.CreateInvitation(userId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            organization.Invitations.Count.Should().Be(1);
            result.Value.Id.Should().NotBeEmpty();
            result.Value.OrganizationId.Should().Be(organization.Id);
            result.Value.UserId.Should().Be(userId);
        }
    }

    [Fact]
    public void CreateInvitation_ShouldReturnOk_WhenThereExistsNonPendingInvitationForThisUser()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var userId = Guid.NewGuid();
        var invitation = organization.CreateInvitation(userId).Value;
        organization.DeclineInvitation(invitation.Id);

        var result = organization.CreateInvitation(userId);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldReturnOk_WhenThereExistsPendingInvitationForAnotherUser()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;
        organization.DeclineInvitation(invitation.Id);

        var result = organization.CreateInvitation(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenUserIsAlreadyAMember()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var result = organization.CreateInvitation(ownerId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenThereAlreadyExistsAPendingInvitationForThisUser()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        _ = organization.CreateInvitation(userId);
        var result = organization.CreateInvitation(userId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldAcceptInvitationAndCreateNewMember()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;

        var result = organization.AcceptInvitation(invitation.Id);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(OrganizationInvitationState.Accepted);
            result.Value.Should().NotBeNull();
            organization.Members.Should().HaveCount(2);
        }
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var result = organization.AcceptInvitation(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;
        organization.AcceptInvitation(invitation.Id);

        var result = organization.AcceptInvitation(invitation.Id);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldDeclineInvitation()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;

        var result = organization.DeclineInvitation(invitation.Id);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(OrganizationInvitationState.Declined);
        }
    }

    [Fact]
    public void DeclineInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var result = organization.DeclineInvitation(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;
        organization.DeclineInvitation(invitation.Id);

        var result = organization.DeclineInvitation(invitation.Id);

        result.IsFailed.Should().BeTrue();
    }
}
