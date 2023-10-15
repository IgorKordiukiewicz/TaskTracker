﻿using Domain.Common;
using Domain.Errors;
using FluentResults;

namespace Domain.Tasks;

// TODO: what would happen if you tried to remove a status that was in use by a task?
// TODO: move to separate folder?
// TODO: Add TaskTransitionStatus Value object
public class Workflow : Entity, IAggregateRoot
{
    public Guid ProjectId { get; set; }

    private readonly List<TaskStatus> _statuses = new();
    public IReadOnlyList<TaskStatus> Statuses => _statuses.AsReadOnly();

    private readonly List<TaskStatusTransition> _transitions = new();
    public IReadOnlyList<TaskStatusTransition> Transitions => _transitions.AsReadOnly();

    private Workflow(Guid projectId)
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
    }

    public static Workflow Create(Guid projectId)
    {
        var toDoId = Guid.NewGuid();
        var inProgressId = Guid.NewGuid();
        var doneId = Guid.NewGuid();

        var workflow = new Workflow(projectId);
        workflow._statuses.Add(TaskStatus.Create(toDoId, "ToDo", 0, true));
        workflow._statuses.Add(TaskStatus.Create(inProgressId, "In Progress", 1));
        workflow._statuses.Add(TaskStatus.Create(doneId, "Done", 2));

        workflow._transitions.Add(new TaskStatusTransition(toDoId, inProgressId));
        workflow._transitions.Add(new TaskStatusTransition(inProgressId, toDoId));
        workflow._transitions.Add(new TaskStatusTransition(inProgressId, doneId));
        workflow._transitions.Add(new TaskStatusTransition(doneId, inProgressId));

        return workflow;
    }

    public bool DoesStatusExist(Guid statusId)
        => _statuses.Any(x => x.Id == statusId);

    public bool CanTransitionTo(Guid currentStatusId, Guid newStatusId)
        => DoesTransitionExist(currentStatusId, newStatusId);

    public Result AddStatus(string name)
    {
        if(_statuses.Any(x => x.Name.ToLower() == name.ToLower()))
        {
            return Result.Fail(new DomainError("Status with this name already exists."));
        }

        var displayOrder = _statuses.MaxBy(x => x.DisplayOrder)!.DisplayOrder + 1;

        _statuses.Add(TaskStatus.Create(Guid.NewGuid(), name, displayOrder));

        return Result.Ok();
    }

    public Result AddTransition(Guid fromStatusId, Guid toStatusId)
    {
        if (!_statuses.Any(x => x.Id == fromStatusId) || !_statuses.Any(x => x.Id == toStatusId))
        {
            return Result.Fail(new DomainError("One of the statuses does not exist."));
        }

        if(DoesTransitionExist(fromStatusId, toStatusId))
        {
            return Result.Fail(new DomainError("Transition already exists."));
        }

        _transitions.Add(new TaskStatusTransition(fromStatusId, toStatusId));
        return Result.Ok();
    }

    public Result DeleteStatus(Guid statusId)
    {
        var status = _statuses.SingleOrDefault(x => x.Id == statusId);
        if(status is null)
        {
            return Result.Fail(new DomainError("Status with this ID does not exist."));
        }

        if(status.Initial)
        {
            return Result.Fail(new DomainError("Initial status can't be deleted."));
        }

        _statuses.Remove(status);
        _transitions.RemoveAll(x => x.FromStatusId == statusId || x.ToStatusId == statusId);

        return Result.Ok();
    }

    private bool DoesTransitionExist(Guid fromStatusId, Guid toStatusId)
        => _transitions.Any(x => x.FromStatusId == fromStatusId && x.ToStatusId == toStatusId);
}