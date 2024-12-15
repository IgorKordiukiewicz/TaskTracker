using Domain.Projects;

namespace Application.Features.Projects;

public record UpdateProjectMemberRoleCommand(Guid ProjectId, UpdateMemberRoleDto Model) : IRequest<Result>;

internal class UpdateProjectMemberRoleCommandValidator : AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class UpdateProjectMemberRoleHandler(IRepository<Project> projectRepository) 
    : IRequestHandler<UpdateProjectMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.UpdateMemberRole(request.Model.MemberId, request.Model.RoleId, project.Members);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}
