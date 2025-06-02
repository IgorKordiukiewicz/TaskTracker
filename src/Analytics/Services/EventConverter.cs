using Analytics.Infrastructure.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Events;
using System.Text.Json;

namespace Analytics.Services;

public static class EventConverter
{
    public static DomainEvent ToDomainEvent(this Event @event)
       => @event.Type switch
       {
           EventType.TaskCreated => DeserializeDomainEvent<TaskCreated>(@event),
           EventType.TaskAssigneeUpdated => DeserializeDomainEvent<TaskAssigneeUpdated>(@event),
           EventType.TaskCommentAdded => DeserializeDomainEvent<TaskCommentAdded>(@event),
           EventType.TaskEstimatedTimeUpdated => DeserializeDomainEvent<TaskEstimatedTimeUpdated>(@event),
           EventType.TaskPriorityUpdated => DeserializeDomainEvent<TaskPriorityUpdated>(@event),
           EventType.TaskStatusUpdated => DeserializeDomainEvent<TaskStatusUpdated>(@event),
           EventType.TaskTimeLogged => DeserializeDomainEvent<TaskTimeLogged>(@event),

           EventType.ProjectCreated => DeserializeDomainEvent<ProjectCreated>(@event),
           EventType.ProjectMemberLeft => DeserializeDomainEvent<ProjectMemberLeft>(@event),
           EventType.ProjectMemberRemoved => DeserializeDomainEvent<ProjectMemberRemoved>(@event),
           EventType.ProjectInvitationCreated => DeserializeDomainEvent<ProjectInvitationCreated>(@event),
           EventType.ProjectInvitationAccepted => DeserializeDomainEvent<ProjectInvitationAccepted>(@event),
           EventType.ProjectInvitationExpired => DeserializeDomainEvent<ProjectInvitationExpired>(@event),
           EventType.ProjectInvitationCanceled => DeserializeDomainEvent<ProjectInvitationCanceled>(@event),
           EventType.ProjectInvitationDeclined => DeserializeDomainEvent<ProjectInvitationDeclined>(@event),

           _ => throw new ArgumentOutOfRangeException($"Event type {@event.Type} is not valid.")
       };

    public static Event ToEvent(this DomainEvent domainEvent)
    {
        var details = domainEvent switch
        {
            TaskCreated taskCreated => JsonSerializer.Serialize(taskCreated),
            TaskAssigneeUpdated taskAssigneeUpdated => JsonSerializer.Serialize(taskAssigneeUpdated),
            TaskCommentAdded taskCommentAdded => JsonSerializer.Serialize(taskCommentAdded),
            TaskEstimatedTimeUpdated taskEstimatedTimeUpdated => JsonSerializer.Serialize(taskEstimatedTimeUpdated),
            TaskPriorityUpdated taskPriorityUpdated => JsonSerializer.Serialize(taskPriorityUpdated),
            TaskStatusUpdated taskStatusUpdated => JsonSerializer.Serialize(taskStatusUpdated),
            TaskTimeLogged taskTimeLogged => JsonSerializer.Serialize(taskTimeLogged),

            ProjectCreated projectCreated => JsonSerializer.Serialize(projectCreated),
            ProjectMemberLeft projectMemberLeft => JsonSerializer.Serialize(projectMemberLeft),
            ProjectMemberRemoved projectMemberRemoved => JsonSerializer.Serialize(projectMemberRemoved),
            ProjectInvitationCreated projectInvitationCreated => JsonSerializer.Serialize(projectInvitationCreated),
            ProjectInvitationAccepted projectInvitationAccepted => JsonSerializer.Serialize(projectInvitationAccepted),
            ProjectInvitationExpired projectInvitationExpired => JsonSerializer.Serialize(projectInvitationExpired),
            ProjectInvitationCanceled projectInvitationCanceled => JsonSerializer.Serialize(projectInvitationCanceled),
            ProjectInvitationDeclined projectInvitationDeclined => JsonSerializer.Serialize(projectInvitationDeclined),

            _ => throw new ArgumentOutOfRangeException($"Event type {domainEvent.Type} is not valid.")
        };

        return new Event()
        {
            ProjectId = domainEvent.ProjectId,
            Type = domainEvent.Type,
            Details = details,
            OccurredAt = domainEvent.OccurredAt
        };
    }

    private static TDomainEvent DeserializeDomainEvent<TDomainEvent>(Event @event)
        where TDomainEvent : DomainEvent
    {
        return JsonSerializer.Deserialize<TDomainEvent>(@event.Details)!;
    }
}
