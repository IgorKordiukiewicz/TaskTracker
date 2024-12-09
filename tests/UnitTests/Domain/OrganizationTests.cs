using Domain.Common;
using Domain.Organizations;

namespace UnitTests.Domain;

public class OrganizationTests
{
    [Fact]
    public void Create_ShouldCreateOrganizationWithOwnerMemberAndDefaultRoles()
    {
        var name = "OrgName";
        var ownerId = Guid.NewGuid();

        var result = Organization.Create(name, ownerId);

        using (new AssertionScope())
        {
            result.Name.Should().Be(name);
            result.OwnerId.Should().Be(ownerId);
            result.Id.Should().NotBeEmpty();

            // Ensure default roles are created
            result.Roles.Count.Should().Be(3);
            result.Roles.Select(x => x.Type).Should().BeEquivalentTo(new RoleType[] { RoleType.Owner, RoleType.Admin, RoleType.ReadOnly });
            var ownerRoleId = result.Roles.First(x => x.Type == RoleType.Owner).Id;

            result.Members.Count.Should().Be(1);
            result.Members[0].UserId.Should().Be(ownerId);
            result.Members[0].RoleId.Should().Be(ownerRoleId);
        }
    }

    [Fact]
    public void CreateInvitation_ShouldCreateInvitationWithCorrectExpirationDate()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        var now = new DateTime(2024, 12, 1);
        var result = organization.CreateInvitation(userId, now, 10);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            organization.Invitations.Count.Should().Be(1);
            result.Value.Id.Should().NotBeEmpty();
            result.Value.OrganizationId.Should().Be(organization.Id);
            result.Value.UserId.Should().Be(userId);
            result.Value.ExpirationDate.Should().Be(new DateTime(2024, 12, 11));
        }
    }

    [Fact]
    public void CreateInvitation_ShouldReturnOk_WhenThereExistsNonPendingInvitationForThisUser()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var userId = Guid.NewGuid();
        var invitation = organization.CreateInvitation(userId, DateTime.Now).Value;
        organization.DeclineInvitation(invitation.Id, DateTime.Now);

        var result = organization.CreateInvitation(userId, DateTime.Now);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldReturnOk_WhenThereExistsPendingInvitationForAnotherUser()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        organization.DeclineInvitation(invitation.Id, DateTime.Now);

        var result = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenUserIsAlreadyAMember()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var result = organization.CreateInvitation(ownerId, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenThereAlreadyExistsAPendingInvitationForThisUser()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        _ = organization.CreateInvitation(userId, DateTime.Now);
        var result = organization.CreateInvitation(userId, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldAcceptInvitationAndCreateNewMember()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var expectedRoleId = organization.Roles.First(x => x.Type == RoleType.ReadOnly).Id;
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = organization.AcceptInvitation(invitation.Id, DateTime.Now);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(OrganizationInvitationState.Accepted);
            result.Value.Should().NotBeNull();
            result.Value.RoleId.Should().Be(expectedRoleId);
            organization.Members.Should().HaveCount(2);
        }
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var result = organization.AcceptInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);

        var result = organization.AcceptInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldDeclineInvitation()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = organization.DeclineInvitation(invitation.Id, DateTime.Now);

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

        var result = organization.DeclineInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        organization.DeclineInvitation(invitation.Id, DateTime.Now);

        var result = organization.DeclineInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CancelInvitation_ShouldCancelInvitation()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = organization.CancelInvitation(invitation.Id, DateTime.Now);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(OrganizationInvitationState.Canceled);
        }
    }

    [Fact]
    public void CancelInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var result = organization.CancelInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CancelInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        organization.CancelInvitation(invitation.Id, DateTime.Now);

        var result = organization.CancelInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void ExpireInvitations_ShouldExpireInvitations_WithExpirationDatePastCurrentAndPendingState()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var now = new DateTime(2024, 12, 2);
        var invitation1 = organization.CreateInvitation(Guid.NewGuid(), now).Value; // No expiration date
        var invitation2 = organization.CreateInvitation(Guid.NewGuid(), now, 10).Value; // Not in pending state
        invitation2.Cancel(now);
        var invitation3 = organization.CreateInvitation(Guid.NewGuid(), now, 20).Value; // Not yet expired
        var invitation4 = organization.CreateInvitation(Guid.NewGuid(), now, 10).Value; // Expired

        organization.ExpireInvitations(now.AddDays(15));

        var expiredInvitations = organization.Invitations.Where(x => x.State == OrganizationInvitationState.Expired);
        expiredInvitations.Should().BeEquivalentTo([invitation4]);
    }

    [Fact]
    public void RemoveMember_ShouldFail_WhenMemberDoesNotExist()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());

        var result = organization.RemoveMember(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldFail_WhenMemberIsOwner()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var member = organization.Members[0];

        var result = organization.RemoveMember(member.Id);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMember_WhenMemberExistsAndIsNotTheOwner()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var userId = Guid.NewGuid();
        var invitation = organization.CreateInvitation(userId, DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);
        var memberId = organization.Members.First(x => x.UserId == userId).Id;

        var result = organization.RemoveMember(memberId);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            organization.Members.Count.Should().Be(1);
        }
    }

    [Fact]
    public void Leave_ShouldFail_WhenUserIsOwner()
    {
        var userId = Guid.NewGuid();
        var organization = Organization.Create("Name", userId);

        var result = organization.Leave(userId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void Leave_ShouldRemoveMember_WhenUserIsNotOwner()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var userId = Guid.NewGuid();
        var invitation = organization.CreateInvitation(userId, DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);

        var result = organization.Leave(userId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            organization.Members.Count.Should().Be(1);
        }    
    }

    [Fact]
    public void GetMemberManagers_ShouldReturnAllMembersThatCanEditMembers()
    {
        var ownerId = Guid.NewGuid();
        var organization = Organization.Create("Name", ownerId);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var invitation1 = organization.CreateInvitation(userId1, DateTime.Now).Value;
        var invitation2 = organization.CreateInvitation(userId2, DateTime.Now).Value;
        var member1 = organization.AcceptInvitation(invitation1.Id, DateTime.Now).Value;
        var member2 = organization.AcceptInvitation(invitation2.Id, DateTime.Now).Value;
        var adminRoleId = organization.Roles.First(x => x.Type == RoleType.Admin).Id;
        _ = organization.RolesManager.UpdateMemberRole(member1.Id, adminRoleId, organization.Members);

        var result = organization.GetMemberManagers();

        result.Select(x => x.UserId).Should().BeEquivalentTo(new[] { ownerId, userId1 });
    }

    [Fact]
    public void Member_UpdateRole_ShouldUpdateRoleId()
    {
        var member = OrganizationMember.Create(Guid.NewGuid(), Guid.NewGuid());
        var newRoleId = Guid.NewGuid();

        member.UpdateRole(newRoleId);

        member.RoleId.Should().Be(newRoleId);
    }
}
