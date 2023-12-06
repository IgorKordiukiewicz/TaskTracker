using Application.Features.Organizations;
using Application.Features.Projects;
using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Shared.Enums;
using Shared.ViewModels;
using Web.Client.Pages.Organization;

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
        var result = await _fixture.SendRequest(new CreateProjectCommand(Guid.NewGuid(), "123", new("project")));

        result.IsFailed.Should().BeTrue();
    }


    [Fact]
    public async Task Create_ShouldFail_WhenProjectWithSameNameAlreadyExistsWithinOrganization()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new CreateProjectCommand(project.OrganizationId, "123", new(project.Name)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Create_ShouldCreateNewProjectAndWorkflow_WhenValidationPassed()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();

        var result = await _fixture.SendRequest(new CreateProjectCommand(organization.Id, user.AuthenticationId, new("project")));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            var project = await _fixture.FirstAsync<Project>(x => x.Id == result.Value);
            project.Should().NotBeNull();
            var workflow = await _fixture.FirstAsync<Domain.Workflows.Workflow>(x => x.ProjectId == result.Value);
            workflow.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetForOrganization_ShouldFail_WhenOrganizationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectsForOrganizationQuery(Guid.NewGuid(), "123"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetForOrganization_ShouldReturnAListOfProjectsUserIsAMemberOfInGivenOrganization()
    {
        var user1 = User.Create("123", "user1", "firstName", "lastName");
        var user2 = User.Create("456", "user2", "firstName", "lastName");
        var organization = Organization.Create("org", user1.Id);
        var invitation = organization.CreateInvitation(user2.Id).Value;
        _ = organization.AcceptInvitation(invitation.Id);
        var project1 = Project.Create("project1", organization.Id, user1.Id);
        var project2 = Project.Create("project2", organization.Id, user1.Id);
        var project3 = Project.Create("project3", organization.Id, user2.Id);
        await _fixture.SeedDb(db =>
        {
            db.AddRange(user1, user2);
            db.Add(organization);
            db.AddRange(project1, project2, project3);
        });

        var result = await _fixture.SendRequest(new GetProjectsForOrganizationQuery(organization.Id, "123"));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Projects.Should().BeEquivalentTo(new[]
            {
                new ProjectVM
                {
                    Id = project1.Id,
                    Name = project1.Name
                },
                new ProjectVM
                {
                    Id = project2.Id,
                    Name = project2.Name
                }
            });
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
        var user1 = User.Create("123", "user1", "firstName", "lastName");
        var user2 = User.Create("1234", "user2", "firstName", "lastName");
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
    public async Task GetMembers_ShouldReturnProjectMembers()
    {
        var project = (await _factory.CreateProjects())[0];
        var user = await _fixture.FirstAsync<User>();

        var result = await _fixture.SendRequest(new GetProjectMembersQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Members.Should().BeEquivalentTo(new[]
            {
                new ProjectMemberVM(project.Members[0].Id, user.Id, user.FullName, project.Members[0].RoleId)
            });
        }
    }

    [Fact]
    public async Task RemoveMember_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveMember_ShuldRemoveMember_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];

        var membersBefore = await _fixture.CountAsync<ProjectMember>();

        var result = await _fixture.SendRequest(new RemoveProjectMemberCommand(project.Id, project.Members[0].Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectMember>()).Should().Be(membersBefore - 1);
        }
    }

    [Fact]
    public async Task GetProjectOrganization_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectOrganizationQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetProjectOrganization_ShouldReturnProjectsOrganizationData_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new GetProjectOrganizationQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.OrganizationId.Should().Be(project.OrganizationId);
        }
    }

    [Fact]
    public async Task GetNavData_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectNavDataQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetNavData_ShouldReturnNavData_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var organization = await _fixture.FirstAsync<Organization>();

        var result = await _fixture.SendRequest(new GetProjectNavDataQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(new ProjectNavigationVM(new(project.Id, project.Name), new(organization.Id, organization.Name)));
        }
    }

    [Fact]
    public async Task GetRoles_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectRolesQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetRoles_ShouldReturnRoles_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        // TODO: create 1 custom role

        var expectedRoles = new List<RoleVM<ProjectPermissions>>();
        foreach (var role in project.Roles)
        {
            expectedRoles.Add(new()
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions,
                Modifiable = role.Type == RoleType.Custom
            });
        }

        var result = await _fixture.SendRequest(new GetProjectRolesQuery(project.Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Roles.Should().BeEquivalentTo(expectedRoles);
        }
    }

    [Fact]
    public async Task CreateRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new CreateProjectRoleCommand(Guid.NewGuid(), new("abc", ProjectPermissions.CreateTasks)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRole_ShouldAddNewRole_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var rolesCountBefore = await _fixture.CountAsync<ProjectRole>();

        var result = await _fixture.SendRequest(new CreateProjectRoleCommand(project.Id, new("abc", ProjectPermissions.CreateTasks)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectRole>()).Should().Be(rolesCountBefore + 1);
        }
    }

    [Fact]
    public async Task DeleteRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new DeleteProjectRoleCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRole_ShouldDeleteRole_WhenPprojectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var rolesCountBefore = await _fixture.CountAsync<ProjectRole>();

        var result = await _fixture.SendRequest(new DeleteProjectRoleCommand(project.Id, project.Roles.First(x => x.Name == roleName).Id));

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.CountAsync<ProjectRole>()).Should().Be(rolesCountBefore - 1);
        }
    }

    [Fact]
    public async Task UpdateRoleName_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectRoleNameCommand(Guid.NewGuid(), Guid.NewGuid(), new("abc")));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRoleName_ShouldUpdateRoleName_WhenProjectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var newName = roleName + "A";
        var roleId = project.Roles.First(x => x.Name == roleName).Id;

        var result = await _fixture.SendRequest(new UpdateProjectRoleNameCommand(project.Id, roleId, new(newName)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectRole>(x => x.Id == roleId)).Name.Should().Be(newName);
        }
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectMemberRoleCommand(Guid.NewGuid(), Guid.NewGuid(), new(Guid.NewGuid())));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldUpdateMemberRoleId_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];
        var member = project.Members[0];
        var newRoleId = project.Roles.First(x => x.Id != member.RoleId).Id;

        var result = await _fixture.SendRequest(new UpdateProjectMemberRoleCommand(project.Id, member.Id, new(newRoleId)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectMember>(x => x.Id == member.Id)).RoleId.Should().Be(newRoleId);
        }
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new UpdateProjectRolePermissionsCommand(Guid.NewGuid(), Guid.NewGuid(), new(ProjectPermissions.None)));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRolePermissions_ShouldUpdatePermissions_WhenProjectExists()
    {
        var (project, roleName) = await CreateProjectWithCustomRole();
        var roleId = project.Roles.First(x => x.Name == roleName).Id;
        var newPermissions = ProjectPermissions.CreateTasks | ProjectPermissions.AddComments;

        var result = await _fixture.SendRequest(new UpdateProjectRolePermissionsCommand(project.Id, roleId, new(newPermissions)));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            (await _fixture.FirstAsync<ProjectRole>(x => x.Id == roleId)).Permissions.Should().Be(newPermissions);
        }
    }

    [Fact]
    public async Task GetProjectSettings_ShouldFail_WhenProjectDoesNotExist()
    {
        var result = await _fixture.SendRequest(new GetProjectSettingsQuery(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GetProjectSettings_ShouldReturnSettings_WhenProjectExists()
    {
        var project = (await _factory.CreateProjects())[0];

        var result = await _fixture.SendRequest(new GetProjectSettingsQuery(project.Id));

        using(new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(new ProjectSettingsVM(project.Name));
        }
    }

    private async Task<(Project Project, string RoleName)> CreateProjectWithCustomRole()
    {
        var organization = (await _factory.CreateOrganizations())[0];
        var user = await _fixture.FirstAsync<User>();
        var project = Project.Create("abc", organization.Id, user.Id);
        var roleName = "abc";
        _ = project.RolesManager.AddRole(roleName, ProjectPermissions.CreateTasks);

        await _fixture.SeedDb(db =>
        {
            db.Add(project);
        });

        return (project, roleName);
    }
}
