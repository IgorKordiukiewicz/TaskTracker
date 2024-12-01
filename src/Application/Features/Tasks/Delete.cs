using Infrastructure.Extensions;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Result>;

internal class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public DeleteTaskHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        return await _dbContext.ExecuteTransaction(async () =>
        {
            await _dbContext.Tasks.DeleteAll(x => x.Id == request.TaskId, cancellationToken);
        });
    }
}
