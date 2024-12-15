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

internal class DeleteOrganizationHandler(AppDbContext dbContext) 
    : IRequestHandler<DeleteOrganizationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var projectsIds = await dbContext.Projects.Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return await dbContext.ExecuteTransaction(async () =>
        {
            await dbContext.Organizations.DeleteAll(x => x.Id == request.OrganizationId, cancellationToken);
            await dbContext.OrganizationInvitations.DeleteAll(x => x.OrganizationId == request.OrganizationId, cancellationToken);
            await dbContext.Projects.DeleteAll(x => projectsIds.Contains(x.Id), cancellationToken);
            await dbContext.Workflows.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
            await dbContext.Tasks.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
            await dbContext.TaskRelationshipManagers.DeleteAll(x => projectsIds.Contains(x.ProjectId), cancellationToken);
        });
    }
}
