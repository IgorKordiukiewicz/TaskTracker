using Domain.Events;
using Domain.Projects;

namespace UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Create_ShouldCreateProjectWithOwnerMemberAndDefaultRoles()
    {
        var name = "project";
        var userId = Guid.NewGuid();
        var result = Project.Create(name, userId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(name);

            // Ensure default roles are created
            result.Roles.Count.Should().Be(3);
            result.Roles.Select(x => x.Type).Should().BeEquivalentTo([RoleType.Owner, RoleType.Admin, RoleType.ReadOnly]);
            var ownerRoleId = result.Roles.First(x => x.Type == RoleType.Owner).Id;

            result.Members.Count.Should().Be(1);
            result.Members[0].UserId.Should().Be(userId);
            result.Members[0].RoleId.Should().Be(ownerRoleId);

            result.HasDomainEvents<ProjectCreated>().Should().BeTrue();
        }
    }

    [Fact]
    public void CreateInvitation_ShouldCreateInvitationWithCorrectExpirationDate()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        var now = new DateTime(2024, 12, 1);
        var result = project.CreateInvitation(userId, now, 10);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            project.Invitations.Count.Should().Be(1);
            result.Value.Id.Should().NotBeEmpty();
            result.Value.ProjectId.Should().Be(project.Id);
            result.Value.UserId.Should().Be(userId);
            result.Value.ExpirationDate.Should().Be(new DateTime(2024, 12, 11));
            project.HasDomainEvents<ProjectInvitationCreated>().Should().BeTrue();
        }
    }

    [Fact]
    public void CreateInvitation_ShouldReturnOk_WhenThereExistsNonPendingInvitationForThisUser()
    {
        var project = Project.Create("Name", Guid.NewGuid());

        var userId = Guid.NewGuid();
        var invitation = project.CreateInvitation(userId, DateTime.Now).Value;
        project.DeclineInvitation(invitation.Id, DateTime.Now);

        var result = project.CreateInvitation(userId, DateTime.Now);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenUserIsAlreadyAMember()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Name", ownerId);

        var result = project.CreateInvitation(ownerId, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CreateInvitation_ShouldFail_WhenThereAlreadyExistsAPendingInvitationForThisUser()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Name", ownerId);

        var userId = Guid.NewGuid();
        _ = project.CreateInvitation(userId, DateTime.Now);
        var result = project.CreateInvitation(userId, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldAcceptInvitationAndCreateNewMember()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var expectedRoleId = project.Roles.First(x => x.Type == RoleType.ReadOnly).Id;
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = project.AcceptInvitation(invitation.Id, DateTime.Now);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(ProjectInvitationState.Accepted);
            result.Value.Should().NotBeNull();
            result.Value.RoleId.Should().Be(expectedRoleId);
            project.Members.Should().HaveCount(2);
            project.HasDomainEvents<ProjectInvitationAccepted>().Should().BeTrue();
        }
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var project = Project.Create("Name", Guid.NewGuid());

        var result = project.AcceptInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AcceptInvitation_ShouldFail_WhenInvitationIsNotPending()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        project.AcceptInvitation(invitation.Id, DateTime.Now);

        var result = project.AcceptInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldDeclineInvitation()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = project.DeclineInvitation(invitation.Id, DateTime.Now);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(ProjectInvitationState.Declined);
            project.HasDomainEvents<ProjectInvitationDeclined>().Should().BeTrue();
        }
    }

    [Fact]
    public void DeclineInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var project = Project.Create("Name", Guid.NewGuid());

        var result = project.DeclineInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void DeclineInvitation_ShouldFail_WhenInvitationIsNotPending()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        project.DeclineInvitation(invitation.Id, DateTime.Now);

        var result = project.DeclineInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CancelInvitation_ShouldCancelInvitation()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;

        var result = project.CancelInvitation(invitation.Id, DateTime.Now);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            invitation.State.Should().Be(ProjectInvitationState.Canceled);
            project.HasDomainEvents<ProjectInvitationCanceled>().Should().BeTrue();
        }
    }

    [Fact]
    public void CancelInvitation_ShouldFail_WhenInvitationDoesNotExist()
    {
        var project = Project.Create("Name", Guid.NewGuid());

        var result = project.CancelInvitation(Guid.NewGuid(), DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void CancelInvitation_ShouldFail_WhenInvitationIsNotPending()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var invitation = project.CreateInvitation(Guid.NewGuid(), DateTime.Now).Value;
        project.CancelInvitation(invitation.Id, DateTime.Now);

        var result = project.CancelInvitation(invitation.Id, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void ExpireInvitations_ShouldExpireInvitations_WithExpirationDatePastCurrentAndPendingState()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var now = new DateTime(2024, 12, 2);
        var invitation1 = project.CreateInvitation(Guid.NewGuid(), now).Value; // No expiration date
        var invitation2 = project.CreateInvitation(Guid.NewGuid(), now, 10).Value; // Not in pending state
        invitation2.Cancel(now);
        var invitation3 = project.CreateInvitation(Guid.NewGuid(), now, 20).Value; // Not yet expired
        var invitation4 = project.CreateInvitation(Guid.NewGuid(), now, 10).Value; // Expired

        project.ExpireInvitations(now.AddDays(15));

        using(new AssertionScope())
        {
            var expiredInvitations = project.Invitations.Where(x => x.State == ProjectInvitationState.Expired);
            expiredInvitations.Should().BeEquivalentTo([invitation4]);
            project.HasDomainEvents<ProjectInvitationExpired>().Should().BeTrue();
        }
    }

    [Fact]
    public void RemoveMember_ShouldFail_WhenMemberDoesNotExist()
    {
        var project = Project.Create("name", Guid.NewGuid());

        var result = project.RemoveMember(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldFail_WhenMemberIsOwner()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var member = project.Members[0];

        var result = project.RemoveMember(member.Id);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMember_WhenMemberExists()
    {
        var project = Project.Create("name", Guid.NewGuid());
        var userId = Guid.NewGuid();
        var invitation = project.CreateInvitation(userId, DateTime.Now).Value;
        project.AcceptInvitation(invitation.Id, DateTime.Now);
        var memberId = project.Members.First(x => x.UserId == userId).Id;

        var result = project.RemoveMember(memberId);

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            project.Members.Count.Should().Be(1);
            project.HasDomainEvents<ProjectMemberRemoved>().Should().BeTrue();
        }
    }

    [Fact]
    public void Leave_ShouldFail_WhenUserIsOwner()
    {
        var userId = Guid.NewGuid();
        var project = Project.Create("Name", userId);

        var result = project.Leave(userId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void Leave_ShouldRemoveMember_WhenUserIsNotOwner()
    {
        var project = Project.Create("Name", Guid.NewGuid());
        var userId = Guid.NewGuid();
        var invitation = project.CreateInvitation(userId, DateTime.Now).Value;
        project.AcceptInvitation(invitation.Id, DateTime.Now);

        var result = project.Leave(userId);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            project.Members.Count.Should().Be(1);
            project.HasDomainEvents<ProjectMemberLeft>().Should().BeTrue();
        }
    }

    [Fact]
    public void GetMembersWithEditMembersPermission_ShouldReturnAllMembersWithEditMembersPermission()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Name", ownerId);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var invitation1 = project.CreateInvitation(userId1, DateTime.Now).Value;
        var invitation2 = project.CreateInvitation(userId2, DateTime.Now).Value;
        var member1 = project.AcceptInvitation(invitation1.Id, DateTime.Now).Value;
        var member2 = project.AcceptInvitation(invitation2.Id, DateTime.Now).Value;
        var adminRoleId = project.Roles.First(x => x.Type == RoleType.Admin).Id;
        _ = project.RolesManager.UpdateMemberRole(member1.Id, adminRoleId, project.Members);

        var result = project.GetMembersWithEditMembersPermission();

        result.Select(x => x.UserId).Should().BeEquivalentTo([ownerId, userId1]);
    }

    [Fact]
    public void Member_UpdateRole_ShouldUpdateRoleId()
    {
        var member = ProjectMember.Create(Guid.NewGuid(), Guid.NewGuid());
        var newRoleId = Guid.NewGuid();

        member.UpdateRole(newRoleId);

        member.RoleId.Should().Be(newRoleId);
    }
}
