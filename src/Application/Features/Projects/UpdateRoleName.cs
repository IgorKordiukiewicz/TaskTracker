using Domain.Projects;

namespace Application.Features.Projects;

public record UpdateProjectRoleNameCommand(Guid ProjectId, UpdateRoleNameDto Model) : IRequest<Result>;

internal class UpdateProjectRoleNameCommandValidator : AbstractValidator<UpdateProjectRoleNameCommand>
{
    public UpdateProjectRoleNameCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class UpdateProjectRoleNameHandler(IRepository<Project> projectRepository) 
    : IRequestHandler<UpdateProjectRoleNameCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectRoleNameCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.UpdateRoleName(request.Model.RoleId, request.Model.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}
