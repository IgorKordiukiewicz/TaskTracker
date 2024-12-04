using Application.Common;
using Domain.Projects;
using Infrastructure.Extensions;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record CreateTaskCommand(Guid ProjectId, CreateTaskDto Model) : IRequest<Result<Guid>>;

internal class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.Title).NotEmpty();
        RuleFor(x => x.Model.Description).NotNull();
    }
}

internal class CreateTaskHandler(AppDbContext dbContext, IRepository<Task> taskRepository, ITasksBoardLayoutService tasksBoardLayoutService) 
    : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<Guid>(new NotFoundError<Project>(request.ProjectId));
        }

        Guid? assigneeId = null;
        if(request.Model.AssigneeMemberId is not null)
        {
            var member = (await dbContext.Projects
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleAsync(x => x.Id == request.ProjectId, cancellationToken))
                .Members.SingleOrDefault(x => x.Id == request.Model.AssigneeMemberId);
            if (member is null)
            {
                return Result.Fail(new NotFoundError<ProjectMember>(request.Model.AssigneeMemberId.Value));
            }

            assigneeId = member.UserId;
        }

        var shortId = (await dbContext.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .CountAsync(cancellationToken)) + 1;

        var initialTaskStatus = await dbContext.Workflows
            .Include(x => x.Statuses)
            .Where(x => x.ProjectId == request.ProjectId)
            .SelectMany(x => x.Statuses)
            .FirstAsync(x => x.Initial, cancellationToken);

        var task = Task.Create(shortId, request.ProjectId, request.Model.Title, request.Model.Description, 
            initialTaskStatus.Id, assigneeId, request.Model.Priority);

        var result = await dbContext.ExecuteTransaction(async () =>
        {
            await taskRepository.Add(task, cancellationToken);
            await tasksBoardLayoutService.HandleChanges(task.ProjectId, layout =>
                layout.CreateTask(task.Id, task.StatusId), cancellationToken);
        });
        
        if(result.IsFailed)
        {
            return Result.Fail<Guid>(result.Errors);
        }

        return task.Id;
    }
}
