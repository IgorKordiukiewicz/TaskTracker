using Application.Data.Repositories;
using Application.Errors;
using Domain.Projects;

namespace Application.Features.Projects;

public record UpdateProjectNameCommand(Guid ProjectId, UpdateProjectNameDto Model) : IRequest<Result>;

internal class UpdateProjectNameCommandValidator : AbstractValidator<UpdateProjectNameCommand>
{
    public UpdateProjectNameCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class UpdateProjectNameHandler : IRequestHandler<UpdateProjectNameCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;

    public UpdateProjectNameHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(UpdateProjectNameCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        project.Name = request.Model.Name;
        await _projectRepository.Update(project);

        return Result.Ok();
    }
}
