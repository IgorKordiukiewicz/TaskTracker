using Application.Features.Projects;
using Application.Models.ViewModels;
using Domain.Projects;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Models;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class ProjectsTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public ProjectsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProjectAndRelatedEntities_WhenValidationPassed()
    {
        var user = (await _factory.CreateUsers())[0];

        var result = await _fixture.SendRequest(new CreateProjectCommand(user.Id, new("project")));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
            var workflow = await _fixture.FirstAsync<Workflow>(x => x.ProjectId == result.Value);
            workflow.Should().NotBeNull();
            var taskRelationManager = await _fixture.FirstAsync<Domain.Tasks.TaskRelationManager>(x => x.ProjectId == result.Value);
            taskRelationManager.Should().NotBeNull();
            var tasksBoardLayout = await _fixture.FirstAsync<TasksBoardLayout>(x => x.ProjectId == result.Value);
            tasksBoardLayout.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task CreateInvitation_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateProjectInvitationCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateInvitation_ShouldFail_WhenUserDoesNotExist()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new CreateProjectInvitationCommand(project.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateInvitation_ShouldCreateNewInvitation_WhenProjectAndUserBothExist()
    {
        var project = (await _factory.CreateProjects())[0];
        var newUser = User.Create(Guid.NewGuid(), "newUser", "firstName", "lastName");
        await _fixture.SeedDb(db =>
        {
            db.Add(newUser);
        });

        var result = await _fixture.SendRequest(new CreateProjectInvitationCommand(project.Id, new(newUser.Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectInvitation>()).Should().Be(1);
        }
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenProjectWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AcceptProjectInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldAddNewProjectMemberAndUpdateInvitationState()
    {
        var invitationId = await CreateProjectWithInvitation();

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new AcceptProjectInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore + 1);

            var invitation = await _fixture.FirstAsync<ProjectInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(ProjectInvitationState.Accepted);
        }
    }

    [Fact]
    public async Task DeclineInvitation_ShouldFail_WhenProjectWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeclineProjectInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeclineInvitation_ShouldUpdateInvitationState()
    {
        var invitationId = await CreateProjectWithInvitation();

        var result = await _fixture.SendRequest(new DeclineProjectInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var invitation = await _fixture.FirstAsync<ProjectInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(ProjectInvitationState.Declined);
        }
    }

    [Fact]
    public async Task CancelInvitation_ShouldFail_WhenProjectWithInvitationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CancelProjectInvitationCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CancelInvitation_ShouldUpdateInvitationState()
    {
        var invitationId = await CreateProjectWithInvitation();

        var result = await _fixture.SendRequest(new CancelProjectInvitationCommand(invitationId));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();

            var invitation = await _fixture.FirstAsync<ProjectInvitation>(x => x.Id == invitationId);
            invitation.State.Should().Be(ProjectInvitationState.Canceled);
        }
    }

    [Fact]
    public async Task ExpireInvitations_ShouldExpireAllExpiredInvitations()
    {
        var users = await _factory.CreateUsers(2);
        var project1 = Project.Create("org1", users[0].Id);
        var project2 = Project.Create("org2", users[0].Id);
        var invitationDate = new DateTime(2010, 1, 1);
        var invitation1 = project1.CreateInvitation(users[1].Id, invitationDate, 10);
        var invitation2 = project2.CreateInvitation(users[1].Id, invitationDate, 10);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(project1, project2);
        });

        await _fixture.SendRequest(new ExpireProjectsInvitationsCommand());

        (await _fixture.CountAsync<ProjectInvitation>(x => x.State == ProjectInvitationState.Expired)).Should().Be(2);
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShuldRemoveMember_WhenProjectExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var project = Project.Create("org", user1.Id);
        var invitation = project.CreateInvitation(user2.Id, DateTime.Now).Value;
        var member = project.AcceptInvitation(invitation.Id, DateTime.Now).Value;

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(project.Id, new(member.Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
        }
    }

    [Fact]
    public async Task CreateRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateProjectRoleCommand(Guid.NewGuid(), new("abc", ProjectPermissions.EditTasks)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRole_ShouldAddNewRole_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var rolesCountBefore = await _fixture.CountAsync<MemberRole>();

        var result = await _fixture.SendRequest(new CreateProjectRoleCommand(project.Id, new("abc", ProjectPermissions.EditTasks)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<MemberRole>()).Should().Be(rolesCountBefore + 1);
        }
    }

    [Fact]
    public async Task DeleteRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteProjectRoleCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRole_ShouldDeleteRole_WhenPprojectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var rolesCountBefore = await _fixture.CountAsync<MemberRole>();

        var result = await _fixture.SendRequest(new DeleteProjectRoleCommand(project.Id, new(project.Roles.First(x => x.Name == roleName).Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<MemberRole>()).Should().Be(rolesCountBefore - 1);
        }
    }

    [Fact]
    public async Task UpdateRoleName_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectRoleNameCommand(Guid.NewGuid(), new(Guid.NewGuid(), "abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRoleName_ShouldUpdateRoleName_WhenProjectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var newName = roleName + "A";
        var roleId = project.Roles.First(x => x.Name == roleName).Id;

        var result = await _fixture.SendRequest(new UpdateProjectRoleNameCommand(project.Id, new(roleId, newName)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<MemberRole>(x => x.Id == roleId)).Name.Should().Be(newName);
        }
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectMemberRoleCommand(Guid.NewGuid(), new(Guid.NewGuid(), Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldUpdateMemberRoleId_WhenProjectExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var project = Project.Create("org", user1.Id);
        var invitation = project.CreateInvitation(user2.Id, DateTime.Now).Value;
        var member2 = project.AcceptInvitation(invitation.Id, DateTime.Now).Value;

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(project);
        });

        var newRoleId = project.Roles.First(x => x.Id != member2.RoleId && x.Type != RoleType.Owner).Id;

        var result = await _fixture.SendRequest(new UpdateProjectMemberRoleCommand(project.Id, new(member2.Id, newRoleId)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectMember>(x => x.Id == member2.Id)).RoleId.Should().Be(newRoleId);
        }
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectRolePermissionsCommand(Guid.NewGuid(), new(Guid.NewGuid(), ProjectPermissions.None)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldUpdatePermissions_WhenProjectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var roleId = project.Roles.First(x => x.Name == roleName).Id;
        var newPermissions = ProjectPermissions.EditRoles | ProjectPermissions.EditMembers;

        var result = await _fixture.SendRequest(new UpdateProjectRolePermissionsCommand(project.Id, new(roleId, newPermissions)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<MemberRole>(x => x.Id == roleId)).Permissions.Should().Be(newPermissions);
        }
    }

    [Fact]
    public async Task UpdateProjectName_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectNameCommand(Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProjectName_ShouldSucceedUpdateName_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var newName = project.Name + "A";

        var result = await _fixture.SendRequest(new UpdateProjectNameCommand(project.Id, new(newName)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<Project>(x => x.Id == project.Id)).Name.Should().Be(newName);
        }
    }

    [Fact]
    public async Task DeleteProject_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteProjectCommand(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteProject_ShouldSucceedAndMarkRequiredRootsAsDeleted_WhenProjectExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var project = await _fixture.FirstAsync<Project>(x => x.Id == task.ProjectId);
        var workflowId = await _fixture.FirstAsync<Workflow>(x => x.ProjectId == project.Id);
        var taskRelationManager = await _fixture.FirstAsync<Domain.Tasks.TaskRelationManager>(x => x.ProjectId == project.Id);

        var result = await _fixture.SendRequest(new DeleteProjectCommand(project.Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<Project>(x => x.Id == project.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.Task>(x => x.Id == task.Id)).Should().Be(0);
            (await _fixture.CountAsync<Workflow>(x => x.Id == workflowId.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.TaskRelationManager>(x => x.Id == taskRelationManager.Id)).Should().Be(0);
            (await _fixture.CountAsync<TasksBoardLayout>(x => x.ProjectId == project.Id)).Should().Be(0);
        }
    }

    [Fact]
    public async Task Leave_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new LeaveProjectCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Leave_ShouldRemoveUserFromProject_WhenProjectExists()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var project = Project.Create("org", user1.Id);
        var invitation = project.CreateInvitation(user2.Id, DateTime.Now).Value;
        project.AcceptInvitation(invitation.Id, DateTime.Now);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new LeaveProjectCommand(project.Id, user2.Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
        }
    }

    private async Task<Guid> CreateProjectWithInvitation()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var project = Project.Create("org", user1.Id);
        var invitation = project.CreateInvitation(user2.Id, DateTime.Now).Value;

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(project);
        });

        return invitation.Id;
    }

    private async Task<(Project Project, string RoleName)> CreateProjectWithCustomRole()
    {
        var user = (await _factory.CreateUsers())[0];
        var project = Project.Create("abc", user.Id);
        var roleName = "abc";
        _ = project.RolesManager.AddRole(roleName, ProjectPermissions.EditTasks);

        await _fixture.SeedDb(db =>
        {
            db.Add(project);
        });

        return (project, roleName);
    }
}