﻿using Domain.Tasks;

namespace Application.Features.Tasks;

public record GetTaskActivitiesQuery(Guid TaskId) : IRequest<Result<TaskActivitiesVM>>;

internal class GetTaskActivitiesQueryValidator : AbstractValidator<GetTaskActivitiesQuery>
{
    public GetTaskActivitiesQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskActivitiesHandler(AppDbContext dbContext) 
    : IRequestHandler<GetTaskActivitiesQuery, Result<TaskActivitiesVM>>
{
    public async Task<Result<TaskActivitiesVM>> Handle(GetTaskActivitiesQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId, cancellationToken))
        {
            return Result.Fail<TaskActivitiesVM>(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var activities = await dbContext.TaskActivities
            .AsNoTracking()
            .Where(x => x.TaskId == request.TaskId)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        var userIds = new HashSet<Guid>();
        var statusIds = new HashSet<Guid>();
        foreach(var activity in activities)
        {
            if(activity.Property == TaskProperty.Assignee)
            {
                TryAddIds(userIds, activity);
            }
            else if(activity.Property == TaskProperty.Status)
            {
                TryAddIds(statusIds, activity);
            }
        }

        var userNameById = await dbContext.Users
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v.FullName, cancellationToken);

        var statusNameById = await dbContext.TaskStatuses
            .Where(x => statusIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        var updatedActivities = new List<TaskActivityVM>();
        foreach(var activity in activities)
        {
            var oldValue = activity.OldValue;
            var newValue = activity.NewValue;

            if (activity.Property == TaskProperty.Assignee)
            {
                UpdateValue(ref oldValue, ref newValue, userNameById);
            }
            else if (activity.Property == TaskProperty.Status)
            {
                UpdateValue(ref oldValue, ref newValue, statusNameById);
            }

            updatedActivities.Add(new(activity.Property, oldValue, newValue, activity.OccurredAt));
        }

        return Result.Ok(new TaskActivitiesVM(updatedActivities));
    }

    private static void TryAddIds(HashSet<Guid> ids, TaskActivity activity)
    {
        if (activity.OldValue is not null)
        {
            ids.Add(Guid.Parse(activity.OldValue));
        }

        if (activity.NewValue is not null)
        {
            ids.Add(Guid.Parse(activity.NewValue));
        }
    }

    private static void UpdateValue(ref string? oldValue, ref string? newValue, IReadOnlyDictionary<Guid, string> valueById)
    {
        if (oldValue is not null)
        {
            oldValue = valueById[Guid.Parse(oldValue)];
        }

        if (newValue is not null)
        {
            newValue = valueById[Guid.Parse(newValue)];
        }
    }
}
