using Domain.Projects;

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

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await _dbContext.Projects
                .Where(x => x.Id == request.ProjectId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.Workflows
                .Where(x => x.ProjectId == request.ProjectId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await _dbContext.Tasks
                .Where(x => x.ProjectId == request.ProjectId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));

            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            return Result.Fail(new Error("SQL Transaction failure").CausedBy(ex));
        }

        return Result.Ok();
    }
}
