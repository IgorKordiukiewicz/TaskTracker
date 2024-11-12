using Domain.Projects;

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
        if(!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId))
        {
            return Result.Fail<ProjectMembersVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var roleNameById = await _dbContext.ProjectRoles
            .Where(x => x.ProjectId == request.ProjectId)
            .ToDictionaryAsync(k => k.Id, v => v.Name);

        var members = await _dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .SelectMany(x => x.Members)
            .Join(_dbContext.Users,
            member => member.UserId,
            user => user.Id,
            (member, user) => new ProjectMemberVM
            {
                Id = member.Id,
                UserId = user.Id,
                Name = user.FullName,
                Email = user.Email,
                RoleId = member.RoleId,
                RoleName = roleNameById[member.RoleId],
            })
            .ToListAsync();

        return Result.Ok(new ProjectMembersVM(members));
    }
}
