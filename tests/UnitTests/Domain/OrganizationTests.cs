using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Shared.Enums;

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
            result.Roles.Count.Should().Be(2);
            result.Roles.Select(x => x.Type).Should().BeEquivalentTo(new RoleType[] { RoleType.Admin, RoleType.ReadOnly });
            var adminRoleId = result.Roles.First(x => x.Type == RoleType.Admin).Id;

            result.Members.Count.Should().Be(1);
            result.Members[0].UserId.Should().Be(ownerId);
            result.Members[0].RoleId.Should().Be(adminRoleId);
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
        var expectedRoleId = organization.Roles.First(x => x.Type == RoleType.ReadOnly).Id;
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;

        var result = organization.AcceptInvitation(invitation.Id);

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

    [Fact]
    public void CancelInvitation_ShouldCancelInvitation()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;

        var result = organization.CancelInvitation(invitation.Id);

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

        var result = organization.CancelInvitation(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CancelInvitation_ShouldFail_WhenInvitationisNotPending()
    {
        var organization = Organization.Create("Name", Guid.NewGuid());
        var invitation = organization.CreateInvitation(Guid.NewGuid()).Value;
        organization.CancelInvitation(invitation.Id);

        var result = organization.CancelInvitation(invitation.Id);

        result.IsFailed.Should().BeTrue();
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
        var invitation = organization.CreateInvitation(userId).Value;
        organization.AcceptInvitation(invitation.Id);
        var memberId = organization.Members.First(x => x.UserId == userId).Id;

        var result = organization.RemoveMember(memberId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            organization.Members.Count.Should().Be(1);
        }
    }

    [Fact]
    public void AddRole_ShouldCreateANewRoleWithCorrectPermissions()
    {
        var organization = Organization.Create("name", Guid.NewGuid());
        var rolesCountBefore = organization.Roles.Count;
        var name = "abc";
        var permissions = OrganizationPermissions.Members;

        _ = organization.AddRole(name, permissions);

        using (new AssertionScope())
        {
            organization.Roles.Count.Should().Be(rolesCountBefore + 1);
            var addedRole = organization.Roles.First(x => x.Name == name);
            addedRole.Permissions.Should().Be(permissions);
        }
    }
}
