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

internal class GetProjectSettingsHandler : IRequestHandler<GetProjectSettingsQuery, Result<ProjectSettingsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectSettingsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectSettingsVM>> Handle(GetProjectSettingsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken)) 
        {
            return Result.Fail<ProjectSettingsVM>(new NotFoundError<Project>(request.ProjectId));
        }

        return await _dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => new ProjectSettingsVM(x.Name))
            .FirstAsync(cancellationToken);
    }
}
