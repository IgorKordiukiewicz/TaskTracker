﻿using Application.Features.Projects;
using Application.Models.ViewModels;
using Domain.Common;
using Domain.Organizations;
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
    public async Task Create_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateProjectCommand(Guid.NewGuid(), new(Guid.NewGuid(), "project")));

        result.IsFailed.Should().BeTrue();
    }


    [Fact]
    public async Task Create_ShouldFail_WhenProjectWithSameNameAlreadyExistsWithinOrganization()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new CreateProjectCommand(Guid.NewGuid(), new(project.OrganizationId, project.Name)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProjectAndRelatedEntities_WhenValidationPassed()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();

        var result = await _fixture.SendRequest(new CreateProjectCommand(user.Id, new(organization.Id, "project")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
            var workflow = await _fixture.FirstAsync<Workflow>(x => x.ProjectId == result.Value);
            workflow.Should().NotBeNull();
            var taskRelationshipManager = await _fixture.FirstAsync<Domain.Tasks.TaskRelationshipManager>(x => x.ProjectId == result.Value);
            taskRelationshipManager.Should().NotBeNull();
            var tasksBoardLayout = await _fixture.FirstAsync<TasksBoardLayout>(x => x.ProjectId == result.Value);
            tasksBoardLayout.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task AddMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new AddProjectMemberCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddMember_ShouldFail_WhenUserIsNotAMemberOfProjectsOrganization()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new AddProjectMemberCommand(project.Id, new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddMember_ShouldAddNewMember()
    {
        var user1 = User.Create(Guid.NewGuid(), "user1", "firstName", "lastName");
        var user2 = User.Create(Guid.NewGuid(), "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var project = Project.Create("project", organization.Id, user2.Id);

        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.Add(project);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new AddProjectMemberCommand(project.Id, new(user1.Id)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore + 1);
        }
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShuldRemoveMemberAndUnassignThemFromAllTasks_WhenProjectExists()
    {
        var task = (await _factory.CreateTasks())[0];
        var project = await _fixture.FirstAsync<Project>();
        var member = await _fixture.FirstAsync<ProjectMember>();
        task.UpdateAssignee(member.UserId, DateTime.Now);

        await _fixture.SeedDb(db =>
        {
            db.Tasks.Update(task);
        });

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(project.Id, new(member.Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
            (await _fixture.FirstAsync<Domain.Tasks.Task>(x => x.Id == task.Id)).AssigneeId.Should().BeNull();
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
        var rolesCountBefore = await _fixture.CountAsync<ProjectRole>();

        var result = await _fixture.SendRequest(new CreateProjectRoleCommand(project.Id, new("abc", ProjectPermissions.EditTasks)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectRole>()).Should().Be(rolesCountBefore + 1);
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
        var rolesCountBefore = await _fixture.CountAsync<ProjectRole>();

        var result = await _fixture.SendRequest(new DeleteProjectRoleCommand(project.Id, new(project.Roles.First(x => x.Name == roleName).Id)));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectRole>()).Should().Be(rolesCountBefore - 1);
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

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectRole>(x => x.Id == roleId)).Name.Should().Be(newName);
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
        var project = (await _factory.CreateProjects())[0];
        var member = project.Members[0];
        var newRoleId = project.Roles.First(x => x.Id != member.RoleId).Id;

        var result = await _fixture.SendRequest(new UpdateProjectMemberRoleCommand(project.Id, new(member.Id, newRoleId)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectMember>(x => x.Id == member.Id)).RoleId.Should().Be(newRoleId);
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

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectRole>(x => x.Id == roleId)).Permissions.Should().Be(newPermissions);
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
        var taskRelationshipManager = await _fixture.FirstAsync<Domain.Tasks.TaskRelationshipManager>(x => x.ProjectId == project.Id);

        var result = await _fixture.SendRequest(new DeleteProjectCommand(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<Project>(x => x.Id == project.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.Task>(x => x.Id == task.Id)).Should().Be(0);
            (await _fixture.CountAsync<Workflow>(x => x.Id == workflowId.Id)).Should().Be(0);
            (await _fixture.CountAsync<Domain.Tasks.TaskRelationshipManager>(x => x.Id == taskRelationshipManager.Id)).Should().Be(0);
            (await _fixture.CountAsync<TasksBoardLayout>(x => x.ProjectId == project.Id)).Should().Be(0);
        }
    }

    private async Task<(Project Project, string RoleName)> CreateProjectWithCustomRole()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();
        var project = Project.Create("abc", organization.Id, user.Id);
        var roleName = "abc";
        _ = project.RolesManager.AddRole(roleName, ProjectPermissions.EditTasks);

        await _fixture.SeedDb(db =>
        {
            db.Add(project);
        });

        return (project, roleName);
    }
}
