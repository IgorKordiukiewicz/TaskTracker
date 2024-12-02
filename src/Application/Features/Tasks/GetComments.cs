namespace Application.Features.Tasks;

public record GetTaskCommentsQuery(Guid TaskId) : IRequest<Result<TaskCommentsVM>>;

internal class GetTaskCommentsQueryValidator : AbstractValidator<GetTaskCommentsQuery>
{
    public GetTaskCommentsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskCommentsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetTaskCommentsQuery, Result<TaskCommentsVM>>
{
    public async Task<Result<TaskCommentsVM>> Handle(GetTaskCommentsQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId, cancellationToken)) 
        {
            return Result.Fail<TaskCommentsVM>(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var comments = await dbContext.TaskComments
            .Where(x => x.TaskId == request.TaskId)
            .OrderBy(x => x.CreatedAt)
            .Join(dbContext.Users,
            comment => comment.AuthorId,
            user => user.Id,
            (comment, user) => new TaskCommentVM(comment.Content, user.Id, user.FullName, comment.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Ok(new TaskCommentsVM(comments));
    }
}
