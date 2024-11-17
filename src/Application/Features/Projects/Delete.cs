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

internal class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public DeleteProjectHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId))
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        return await _dbContext.ExecuteTransaction(async () =>
        {
            await _dbContext.Projects.DeleteAll(x => x.Id == request.ProjectId);
            await _dbContext.Workflows.DeleteAll(x => x.ProjectId == request.ProjectId);
            await _dbContext.Tasks.DeleteAll(x => x.ProjectId == request.ProjectId);
            await _dbContext.TaskRelationshipManagers.DeleteAll(x => x.ProjectId == request.ProjectId);
        });
    }
}
