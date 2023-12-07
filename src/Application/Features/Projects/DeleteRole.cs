using Domain.Projects;

namespace Application.Features.Projects;

public record DeleteProjectRoleCommand(Guid ProjectId, Guid RoleId) : IRequest<Result>;

internal class DeleteProjectRoleCommandValidator : AbstractValidator<DeleteProjectRoleCommand>
{
    public DeleteProjectRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

internal class DeleteProjectRoleHandler : IRequestHandler<DeleteProjectRoleCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;

    public DeleteProjectRoleHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(DeleteProjectRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.DeleteRole(request.RoleId, project.Members);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _projectRepository.Update(project);
        return Result.Ok();
    }
}
