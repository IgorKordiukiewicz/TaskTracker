namespace Application.Features.Projects;

public record GetProjectMembersQuery(Guid ProjectId) : IRequest<Result<ProjectMembersVM>>;

internal class GetProjectMembersQueryValidator : AbstractValidator<GetProjectMembersQuery>
{
    public GetProjectMembersQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectMembersHandler : IRequestHandler<GetProjectMembersQuery, Result<ProjectMembersVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectMembersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectMembersVM>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _dbContext.Projects.AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .SelectMany(x => x.Members)
            .Join(_dbContext.Users,
            member => member.UserId,
            user => user.Id,
            (member, user) => new ProjectMemberVM(member.Id, user.Name))
            .ToListAsync();

        return Result.Ok(new ProjectMembersVM(members));
    }
}
