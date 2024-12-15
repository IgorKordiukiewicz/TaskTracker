using Application.Common;
using Azure.Core;
using Infrastructure.Models;
using MediatR;
using System.Data;
using System.Threading;

namespace Application.Features.Tasks;

public record UpdateTaskBoardCommand(UpdateTaskBoardDto Model) : IRequest<Result>;

internal class UpdateTaskBoardCommandValidator : AbstractValidator<UpdateTaskBoardCommand>
{
    public UpdateTaskBoardCommandValidator()
    {
        RuleFor(x => x.Model.ProjectId).NotEmpty();
        RuleFor(x => x.Model.Columns).NotNull();
    }
}

internal class UpdateTaskBoardHandler(AppDbContext dbContext, ITasksBoardLayoutService tasksBoardLayoutService)
    : IRequestHandler<UpdateTaskBoardCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskBoardCommand request, CancellationToken cancellationToken)
    {
        if(!await dbContext.TasksBoardLayouts.AnyAsync(x => x.ProjectId == request.Model.ProjectId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<TasksBoardLayout>($"project ID: {request.Model.ProjectId}"));
        }

        if(!await AllStatusesMatching(request.Model, cancellationToken))
        {
            return Result.Fail(new ApplicationError("Board layout statuses do not match the existing statuses."));
        }

        if(!await AllTasksMatching(request.Model, cancellationToken))
        {
            return Result.Fail(new ApplicationError("Board layout tasks do not match the existing tasks."));
        }

        await tasksBoardLayoutService.HandleChanges(request.Model.ProjectId, layout => layout.Update(request.Model.Columns.Select(x => new TasksBoardColumn()
        {
            StatusId = x.StatusId,
            TasksIds = [.. x.TasksIds]
        }).ToList()), cancellationToken);

        return Result.Ok();
    }

    private async Task<bool> AllStatusesMatching(UpdateTaskBoardDto model, CancellationToken cancellationToken)
    {
        var boardStatusesIds = model.Columns
            .Select(x => x.StatusId)
            .Order();
        var currentStatusesIds = (await dbContext.Workflows
            .Where(x => x.ProjectId == model.ProjectId)
            .SelectMany(x => x.Statuses)
        .Select(x => x.Id)
            .ToListAsync(cancellationToken))
            .Order();
        return boardStatusesIds.SequenceEqual(currentStatusesIds);
    }

    private async Task<bool> AllTasksMatching(UpdateTaskBoardDto model, CancellationToken cancellationToken)
    {
        var boardTasks = model.Columns
            .SelectMany(x => x.TasksIds.Select(xx => $"{x.StatusId}{xx}"))
            .Order();
        var currentTasks = (await dbContext.Tasks
            .Where(x => x.ProjectId == model.ProjectId)
            .Select(x => new { x.Id, x.StatusId })
            .ToListAsync(cancellationToken))
            .Select(x => $"{x.StatusId}{x.Id}")
            .Order();

        return boardTasks.SequenceEqual(currentTasks);
    }
}
