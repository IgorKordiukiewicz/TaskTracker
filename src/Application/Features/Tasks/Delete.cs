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

internal class DeleteTaskHandler(AppDbContext dbContext) 
    : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        return await dbContext.ExecuteTransaction(async () =>
        {
            await dbContext.Tasks.DeleteAll(x => x.Id == request.TaskId, cancellationToken);
        });
    }
}
