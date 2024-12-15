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

internal class UpdateProjectNameHandler(IRepository<Project> projectRepository) 
    : IRequestHandler<UpdateProjectNameCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectNameCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        project.Name = request.Model.Name;
        await projectRepository.Update(project, cancellationToken);

        return Result.Ok();
    }
}
