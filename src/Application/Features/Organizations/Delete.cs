using Domain.Organizations;
using Infrastructure.Extensions;

namespace Application.Features.Organizations;

public record DeleteOrganizationCommand(Guid OrganizationId) : IRequest<Result>;

internal class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommand>
{
    public DeleteOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class DeleteOrganizationHandler : IRequestHandler<DeleteOrganizationCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public DeleteOrganizationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var projectsIds = await _dbContext.Projects.Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => x.Id)
            .ToListAsync();

        return await _dbContext.ExecuteTransaction(async () =>
        {
            await _dbContext.Organizations
               .Where(x => x.Id == request.OrganizationId)
               .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.OrganizationInvitations
                .Where(x => x.OrganizationId == request.OrganizationId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.Projects
                .Where(x => projectsIds.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.Workflows
                .Where(x => projectsIds.Contains(x.ProjectId))
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.Tasks
                .Where(x => projectsIds.Contains(x.ProjectId))
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.TaskRelationshipManagers
                .Where(x => projectsIds.Contains(x.ProjectId))
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));
        });

        return Result.Ok();
    }
}
