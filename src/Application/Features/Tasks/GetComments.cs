using Application.Errors;

namespace Application.Features.Tasks;

public record GetTaskCommentsQuery(Guid TaskId) : IRequest<Result<TaskCommentsVM>>;

internal class GetTaskCommentsQueryValidator : AbstractValidator<GetTaskCommentsQuery>
{
    public GetTaskCommentsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskCommentsHandler : IRequestHandler<GetTaskCommentsQuery, Result<TaskCommentsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetTaskCommentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TaskCommentsVM>> Handle(GetTaskCommentsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId)) 
        {
            return Result.Fail<TaskCommentsVM>(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var comments = await _dbContext.TaskComments
            .Where(x => x.TaskId == request.TaskId)
            .OrderBy(x => x.CreatedAt)
            .Join(_dbContext.Users,
            comment => comment.AuthorId,
            user => user.Id,
            (comment, user) => new TaskCommentVM(comment.Content, user.FullName, comment.CreatedAt))
            .ToListAsync();

        return Result.Ok(new TaskCommentsVM(comments));
    }
}
