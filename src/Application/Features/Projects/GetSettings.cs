using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectSettingsQuery(Guid ProjectId) : IRequest<Result<ProjectSettingsVM>>;

internal class GetProjectSettingsQueryValidator : AbstractValidator<GetProjectSettingsQuery>
{
    public GetProjectSettingsQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectSettingsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectSettingsQuery, Result<ProjectSettingsVM>>
{
    public async Task<Result<ProjectSettingsVM>> Handle(GetProjectSettingsQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken)) 
        {
            return Result.Fail<ProjectSettingsVM>(new NotFoundError<Project>(request.ProjectId));
        }

        return await dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => new ProjectSettingsVM(x.Name))
            .FirstAsync(cancellationToken);
    }
}
