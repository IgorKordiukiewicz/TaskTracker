﻿using Application.Data.Repositories;
using Application.Errors;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record AddTaskCommentCommand(Guid TaskId, string UserAuthenticationId, AddTaskCommentDto Model) : IRequest<Result>;

internal class AddTaskCommentCommandValidator : AbstractValidator<AddTaskCommentCommand>
{
    public AddTaskCommentCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
        RuleFor(x => x.Model.Content).NotEmpty();
    }
}

internal class AddTaskCommentHandler : IRequestHandler<AddTaskCommentCommand, Result>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Task> _taskRepository;

    public AddTaskCommentHandler(AppDbContext dbContext, IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddTaskCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if(task is null)
        {
            return Result.Fail(new ApplicationError("Task with this ID does not exist."));
        }

        var userId = (await _dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.AuthenticationId == request.UserAuthenticationId))?.Id ?? Guid.Empty;

        task.AddComment(request.Model.Content, userId);

        await _taskRepository.Update(task);
        return Result.Ok();
    }
}
