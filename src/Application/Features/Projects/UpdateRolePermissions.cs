using Domain.Projects;

namespace Application.Features.Projects;

public record UpdateProjectRolePermissionsCommand(Guid ProjectId, UpdateRolePermissionsDto Model) : IRequest<Result>;

internal class UpdateProjectRolePermissionsCommandValidator : AbstractValidator<UpdateProjectRolePermissionsCommand>
{
    public UpdateProjectRolePermissionsCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
        RuleFor(x => x.Model.Permissions).NotEmpty();
    }
}

internal class UpdateProjectRolePermissionsHandler(IRepository<Project> projectRepository) 
    : IRequestHandler<UpdateProjectRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.UpdateRolePermissions(request.Model.RoleId, request.Model.Permissions);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}
