using Domain.Projects;

namespace Application.Features.Projects;

public record GetUserProjectPermissionsQuery(Guid UserId, Guid ProjectId) : IRequest<Result<UserProjectPermissionsVM>>;

internal class GetUserProjectPermissionsQueryValidator : AbstractValidator<GetUserProjectPermissionsQuery>
{
    public GetUserProjectPermissionsQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetUserProjectPermissionsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetUserProjectPermissionsQuery, Result<UserProjectPermissionsVM>>
{
    public async Task<Result<UserProjectPermissionsVM>> Handle(GetUserProjectPermissionsQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<UserProjectPermissionsVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var userRoleId = await dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.Id == request.ProjectId)
            .SelectMany(x => x.Members)
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.RoleId)
            .FirstAsync(cancellationToken);

        var permissions = await dbContext.ProjectRoles
            .Where(x => x.ProjectId == request.ProjectId && x.Id == userRoleId)
            .Select(x => x.Permissions)
            .FirstAsync(cancellationToken);

        return new UserProjectPermissionsVM(permissions);
    }
}
