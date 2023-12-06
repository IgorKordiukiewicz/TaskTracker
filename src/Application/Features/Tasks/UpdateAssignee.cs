using Application.Data.Repositories;
using Application.Errors;
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

internal class UpdateTaskAssigneeHandler : IRequestHandler<UpdateTaskAssigneeCommand, Result>
{
    private readonly IRepository<Task> _taskRepository;
    private readonly AppDbContext _dbContext;

    public UpdateTaskAssigneeHandler(IRepository<Task> taskRepository, AppDbContext dbContext)
    {
        _taskRepository = taskRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateTaskAssigneeCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        if(request.Model.MemberId is not null)
        {
            var member = (await _dbContext.Projects
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleAsync(x => x.Id == task.ProjectId))
                .Members.SingleOrDefault(x => x.Id == request.Model.MemberId);
            if (member is null)
            {
                return Result.Fail(new NotFoundError<ProjectMember>(request.Model.MemberId.Value));
            }

            task.UpdateAssignee(member.UserId); // TODO: Store ref to UserId or MemberId ?
        }
        else
        {
            task.Unassign();
        }

        await _taskRepository.Update(task);
        
        return Result.Ok();
    }
}
