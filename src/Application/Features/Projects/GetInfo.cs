using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectInfoQuery(Guid ProjectId) : IRequest<Result<ProjectInfoVM>>;

internal class GetProjectInfoQueryValidator : AbstractValidator<GetProjectInfoQuery>
{
    public GetProjectInfoQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectInfoHandler(AppDbContext dbContext)
    : IRequestHandler<GetProjectInfoQuery, Result<ProjectInfoVM>>
{
    public async Task<Result<ProjectInfoVM>> Handle(GetProjectInfoQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<ProjectInfoVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var projectInfo = await dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => new ProjectInfoVM(x.Id, x.Name))
            .SingleAsync(cancellationToken);

        return projectInfo;
    }
}
