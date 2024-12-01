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
        if (!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var projectsIds = await _dbContext.Projects.Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return await _dbContext.ExecuteTransaction(async () =>
        {
            await _dbContext.Organizations.DeleteAll(x => x.Id == request.OrganizationId, cancellationToken);
            await _dbContext.OrganizationInvitations.DeleteAll(x => x.OrganizationId == request.OrganizationId, cancellationToken);
            await _dbContext.Projects.DeleteAll(x => projectsIds.Contains(x.Id), cancellationToken);
            await _dbContext.Workflows.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
            await _dbContext.Tasks.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
            await _dbContext.TaskRelationshipManagers.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
        });
    }
}
