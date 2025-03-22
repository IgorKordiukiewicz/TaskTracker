using Application.Features.Organizations;
using Application.Models.ViewModels;
using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.Workflows;

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
    public async Task Create_ShouldFail_WhenOrganizationWithSameNameAlreadyExists()
    {
        var organization = (await _factory.CreateOrganizations())[0];

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new(organization.Name), organization.OwnerId));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewOrganization_WhenOwnerExists()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new CreateOrganizationCommand(new("org"), user.Id));

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
        var newUser = User.Create(Guid.NewGuid(), "newUser","firstName", "lastName");
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

            var invitation = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(OrganizationInvitationState.Accepted);
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

        var result = await _fixture.SendRequest(new DeclineOrganizationInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var invitation = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(OrganizationInvitationState.Declined);
        }
    }

    [Fact]
    public async Task CancelInvitation_ShouldFail_WhenOrganizationWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CancelOrganizationInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CancelInvitation_ShouldUpdateInvitationState()
    {
        var invitationId = await CreateOrganizationWithInvitation();

        var result = await _fixture.SendRequest(new CancelOrganizationInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var invitation = await _fixture.FirstAsync<OrganizationInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(OrganizationInvitationState.Canceled);
        }
    }

    [Fact]
    public async Task ExpireInvitations_ShouldExpireAllExpiredInvitations()
    {
        var users = await _factory.CreateUsers(2);
        var org1 = Organization.Create("org1", users[0].Id);
        var org2 = Organization.Create("org2", users[0].Id);
        var invitationDate = new DateTime(2010, 1, 1);
        var invitation1 = org1.CreateInvitation(users[1].Id, invitationDate, 10);
        var invitation2 = org2.CreateInvitation(users[1].Id, invitationDate, 10);
        
        await _fixture.SeedDb(db =>
        {
            db.AddRange(org1, org2);
        });
        
        await _fixture.SendRequest(new ExpireOrganizationsInvitationsCommand());
        
        (await _fixture.CountAsync<OrganizationInvitation>(x => x.State == OrganizationInvitationState.Expired)).Should().Be(2);
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveOrganizationMemberCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShouldRemoveMemberFromOrganizationAndItsProjects_WhenOrganizationExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id, DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);

        var project = Project.Create("proj1", organization.Id, user1.Id);
        project.AddMember(user2.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();
        var projectMembersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveOrganizationMemberCommand(organization.Id, 
            new(organization.Members.First(x => x.UserId == user2.Id).Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationMember>()).Should().Be(membersBefore - 1);
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(projectMembersBefore - 1);
        }
    }

    [Fact]
    public async Task CreateRole_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateOrganizationRoleCommand(Guid.NewGuid(), new("abc", OrganizationPermissions.EditProjects)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRole_ShouldAddNewRole_WhenOrganizationExists()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var rolesCountBefore = await _fixture.CountAsync<OrganizationRole>();

        var result = await _fixture.SendRequest(new CreateOrganizationRoleCommand(organization.Id, new("abc", OrganizationPermissions.EditProjects)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationRole>()).Should().Be(rolesCountBefore + 1);
        }
    }

    [Fact]
    public async Task DeleteRole_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteOrganizationRoleCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRole_ShouldDeleteRole_WhenOrganizationExists()
    {
        var (organization, roleName) = await CreateOrganizationWithCustomRole();
        var rolesCountBefore = await _fixture.CountAsync<OrganizationRole>();

        var result = await _fixture.SendRequest(new DeleteOrganizationRoleCommand(organization.Id, new(organization.Roles.First(x => x.Name == roleName).Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationRole>()).Should().Be(rolesCountBefore - 1);
        }
    }

    [Fact]
    public async Task UpdateRoleName_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateOrganizationRoleNameCommand(Guid.NewGuid(), new(Guid.NewGuid(), "abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRoleName_ShouldUpdateRoleName_WhenOrganizationExists()
    {
        var (organization, roleName) = await CreateOrganizationWithCustomRole();
        var newName = roleName + "A";
        var roleId = organization.Roles.First(x => x.Name == roleName).Id;

        var result = await _fixture.SendRequest(new UpdateOrganizationRoleNameCommand(organization.Id, new(roleId, newName)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<OrganizationRole>(x => x.Id == roleId)).Name.Should().Be(newName);
        }
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateOrganizationMemberRoleCommand(Guid.NewGuid(), new(Guid.NewGuid(), Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldUpdateMemberRoleId_WhenOrganizationExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id, DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);
        var member2 = organization.Members.First(x => x.UserId == user2.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
        });

        var newRoleId = organization.Roles.First(x => x.Id != member2.RoleId && x.Type != RoleType.Owner).Id;

        var result = await _fixture.SendRequest(new UpdateOrganizationMemberRoleCommand(organization.Id, new(member2.Id, newRoleId)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<OrganizationMember>(x => x.Id == member2.Id)).RoleId.Should().Be(newRoleId);
        }
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateOrganizationRolePermissionsCommand(Guid.NewGuid(), new(Guid.NewGuid(), OrganizationPermissions.None)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldUpdatePermissions_WhenOrganizationExists()
    {
        var (organization, roleName) = await CreateOrganizationWithCustomRole();
        var roleId = organization.Roles.First(x => x.Name == roleName).Id;
        var newPermissions = OrganizationPermissions.EditRoles | OrganizationPermissions.EditMembers;

        var result = await _fixture.SendRequest(new UpdateOrganizationRolePermissionsCommand(organization.Id, new(roleId, newPermissions)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<OrganizationRole>(x => x.Id == roleId)).Permissions.Should().Be(newPermissions);
        }
    }

    [Fact]
    public async Task DeleteOrganization_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteOrganizationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteOrganization_ShouldSucceedAndMarkRequiredRootsAsDeleted_WhenOrganizationExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var project = await _fixture.FirstAsync<Project>(x => x.Id == task.ProjectId);
        var workflowId = await _fixture.FirstAsync<Workflow>(x => x.ProjectId == project.Id);
        var taskRelationshipManager = await _fixture.FirstAsync<Domain.Tasks.TaskRelationshipManager>(x => x.ProjectId == project.Id);
        var organization = await _fixture.FirstAsync<Organization>(x => x.Id == project.OrganizationId);

        var user = User.Create(Guid.NewGuid(), "aaa@mail.com", "bbb", "ccc");
        var organizationInvitation = organization.CreateInvitation(user.Id, DateTime.Now).Value;
        await _fixture.SeedDb(db =>
        {
            db.Add(user);
            db.Add(organizationInvitation);
        });

        var result = await _fixture.SendRequest(new DeleteOrganizationCommand(organization.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<Organization>(x => x.Id == organization.Id)).Should().Be(0);
            (await _fixture.CountAsync<OrganizationInvitation>(x => x.OrganizationId == organization.Id)).Should().Be(0);
            (await _fixture.CountAsync<Project>(x => x.Id == project.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.Task>(x => x.Id == task.Id)).Should().Be(0);
            (await _fixture.CountAsync<Workflow>(x => x.Id == workflowId.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.TaskRelationshipManager>(x => x.Id == taskRelationshipManager.Id)).Should().Be(0);
        }
    }

    [Fact]
    public async Task UpdateName_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateOrganizationNameCommand(Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateName_ShouldSucceedAndUpdateName_WhenOrganizationExists()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var newName = organization.Name + "A";

        var result = await _fixture.SendRequest(new UpdateOrganizationNameCommand(organization.Id, new(newName)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Organization>(x => x.Id == organization.Id)).Name.Should().Be(newName);
        }
    }

    [Fact]
    public async Task Leave_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new LeaveOrganizationCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Leave_ShouldRemoveUserFromOrganizationAndItsProjets_WhenOrganizationExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id, DateTime.Now).Value;
        organization.AcceptInvitation(invitation.Id, DateTime.Now);

        var project = Project.Create("proj1", organization.Id, user1.Id);
        project.AddMember(user2.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<OrganizationMember>();
        var projectMembersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveOrganizationMemberCommand(organization.Id,
            new(organization.Members.First(x => x.UserId == user2.Id).Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<OrganizationMember>()).Should().Be(membersBefore - 1);
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(projectMembersBefore - 1);
        }
    }

    private async Task<Guid> CreateOrganizationWithInvitation()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1","firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id, DateTime.Now).Value;

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
        });

        return invitation.Id;
    }

    private async Task<(Organization Organization, string RoleName)> CreateOrganizationWithCustomRole()
    {
        var user = (await _factory.CreateUsers())[0];
        var organization = Organization.Create("org", user.Id);
        var roleName = "abc";
        _ = organization.RolesManager.AddRole(roleName, OrganizationPermissions.EditProjects);

        await _fixture.SeedDb(db =>
        {
            db.Add(organization);
        });

        return (organization, roleName);
    }
}
