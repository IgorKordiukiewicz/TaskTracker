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

internal class GetProjectMembersHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectMembersQuery, Result<ProjectMembersVM>>
{
    public async Task<Result<ProjectMembersVM>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail<ProjectMembersVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var roleNameById = await dbContext.ProjectRoles
            .Where(x => x.ProjectId == request.ProjectId)
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        var members = (await dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .SelectMany(x => x.Members)
            .Join(dbContext.Users,
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
                Owner = user.Id == project.OwnerId
            })
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .ToList();

        return Result.Ok(new ProjectMembersVM(members));
    }
}
