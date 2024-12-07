using Application.Common;
using Domain.Notifications;
using Domain.Projects;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record UpdateTaskAssigneeCommand(Guid TaskId, UpdateTaskAssigneeDto Model) : IRequest<Result>;

internal class UpdateTaskAssigneeCommandValidator : AbstractValidator<UpdateTaskAssigneeCommand>
{
    public UpdateTaskAssigneeCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model).NotNull();
    }
}

internal class UpdateTaskAssigneeHandler(IRepository<Task> taskRepository, AppDbContext dbContext, 
    IJobsService jobsService, IDateTimeProvider dateTimeProvider) 
    : IRequestHandler<UpdateTaskAssigneeCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskAssigneeCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        var oldAssigneeUserId = task.AssigneeId;

        if (request.Model.MemberId is not null)
        {
            var member = (await dbContext.Projects
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleAsync(x => x.Id == task.ProjectId, cancellationToken))
                .Members.SingleOrDefault(x => x.Id == request.Model.MemberId);
            if (member is null)
            {
                return Result.Fail(new NotFoundError<ProjectMember>(request.Model.MemberId.Value));
            }

            task.UpdateAssignee(member.UserId, dateTimeProvider.Now());

            jobsService.EnqueueCreateNotification(NotificationFactory.TaskAssigned(member.UserId, dateTimeProvider.Now(), task.ProjectId, task.ShortId));
        }
        else
        {
            task.Unassign(dateTimeProvider.Now());
        }

        await taskRepository.Update(task, cancellationToken);

        if(oldAssigneeUserId.HasValue)
        {
            jobsService.EnqueueCreateNotification(NotificationFactory.TaskUnassigned(oldAssigneeUserId!.Value, dateTimeProvider.Now(), task.ProjectId, task.ShortId));
        }
        
        return Result.Ok();
    }
}
