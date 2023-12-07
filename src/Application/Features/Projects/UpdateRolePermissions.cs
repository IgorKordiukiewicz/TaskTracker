using Domain.Projects;
using Shared.Enums;

namespace Application.Features.Projects;

public record UpdateProjectRolePermissionsCommand(Guid ProjectId, Guid RoleId, UpdateRolePermissionsDto<ProjectPermissions> Model) : IRequest<Result>;

internal class UpdateProjectRolePermissionsCommandValidator : AbstractValidator<UpdateProjectRolePermissionsCommand>
{
    public UpdateProjectRolePermissionsCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.Model.Permissions).NotEmpty();
    }
}

internal class UpdateProjectRolePermissionsHandler : IRequestHandler<UpdateProjectRolePermissionsCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;

    public UpdateProjectRolePermissionsHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(UpdateProjectRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.UpdateRolePermissions(request.RoleId, request.Model.Permissions);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _projectRepository.Update(project);
        return Result.Ok();
    }
}
