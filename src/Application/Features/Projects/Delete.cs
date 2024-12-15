using Domain.Projects;
using Infrastructure.Extensions;

namespace Application.Features.Projects;

public record DeleteProjectCommand(Guid ProjectId) : IRequest<Result>;

internal class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class DeleteProjectHandler(AppDbContext dbContext) 
    : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        return await dbContext.ExecuteTransaction(async () =>
        {
            await dbContext.Projects.DeleteAll(x => x.Id == request.ProjectId, cancellationToken);
            await dbContext.Workflows.DeleteAll(x => x.ProjectId == request.ProjectId, cancellationToken);
            await dbContext.Tasks.DeleteAll(x => x.ProjectId == request.ProjectId, cancellationToken);
            await dbContext.TaskRelationshipManagers.DeleteAll(x => x.ProjectId == request.ProjectId, cancellationToken);
            await dbContext.TasksBoardLayouts.DeleteAll(x => x.ProjectId == request.ProjectId, cancellationToken);
        });
    }
}
