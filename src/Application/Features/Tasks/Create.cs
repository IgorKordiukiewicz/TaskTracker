﻿using Application.Data.Repositories;
using Application.Errors;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record CreateTaskCommand(Guid ProjectId, CreateTaskDto Model) : IRequest<Result<Guid>>;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.Title).NotEmpty();
        RuleFor(x => x.Model.Description).NotEmpty(); // TODO: Make description optional?
    }
}

internal class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Task> _taskRepository;

    public CreateTaskHandler(AppDbContext dbContext, IRepository<Task> taskRepository)
    {
        _dbContext = dbContext;
        _taskRepository = taskRepository;
    }


    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId))
        {
            return Result.Fail<Guid>(new ApplicationError("Project with this ID does not exist."));
        }

        var shortId = (await _dbContext.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .CountAsync()) + 1;

        var initialTaskStatus = await _dbContext.Workflows
            .Include(x => x.Statuses)
            .Where(x => x.ProjectId == request.ProjectId)
            .SelectMany(x => x.Statuses)
            .FirstAsync(x => x.Initial);

        var task = Task.Create(shortId, request.ProjectId, request.Model.Title, request.Model.Description, initialTaskStatus.Id);

        await _taskRepository.Add(task);

        return task.Id;
    }
}