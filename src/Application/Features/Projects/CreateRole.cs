using Domain.Projects;

namespace Application.Features.Projects;

public record CreateProjectRoleCommand(Guid ProjectId, CreateRoleDto<ProjectPermissions> Model) : IRequest<Result>;

internal class CreateProjectRoleCommandValidator : AbstractValidator<CreateProjectRoleCommand>
{
    public CreateProjectRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
        RuleFor(x => x.Model.Permissions).NotNull(); // TODO: Validate in range
    }
}

internal class CreateProjectRoleHandler : IRequestHandler<CreateProjectRoleCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;

    public CreateProjectRoleHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(CreateProjectRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.AddRole(request.Model.Name, request.Model.Permissions);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}
