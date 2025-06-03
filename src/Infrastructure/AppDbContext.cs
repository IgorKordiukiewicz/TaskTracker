using Analytics.Services;
using Domain.Common;
using Domain.Notifications;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Models;

namespace Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher domainEventDispatcher) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectInvitation> ProjectInvitations { get; set; }
    public DbSet<MemberRole> ProjectRoles { get; set; }

    public DbSet<Domain.Tasks.Task> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<TaskActivity> TaskActivities { get; set; }
    public DbSet<TaskTimeLog> TaskTimeLogs { get; set; }
    public DbSet<TaskRelationshipManager> TaskRelationshipManagers { get; set; }
    public DbSet<TaskHierarchicalRelationship> TaskHierarchicalRelationships { get; set; }

    public DbSet<Domain.Workflows.TaskStatus> TaskStatuses { get; set; }
    public DbSet<TaskStatusTransition> TaskStatusTransitions { get; set; }
    public DbSet<Workflow> Workflows { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<UserPresentationData> UsersPresentationData { get; set; }
    public DbSet<TasksBoardLayout> TasksBoardLayouts { get; set; }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEvents(cancellationToken);

        return result;
    }

    public void AddRemoveChildEntities<TDependent>(IEnumerable<TDependent> actualEntities, IEnumerable<Guid> dbEntities)
        where TDependent : Entity
    {
        var addedEntities = actualEntities.Where(x => !dbEntities.Any(y => y == x.Id));
        Set<TDependent>().AddRange(addedEntities);

        var removedEntitiesIds = dbEntities.Where(x => !actualEntities.Any(y => y.Id == x));
        Set<TDependent>().RemoveRange(Set<TDependent>().Where(x => removedEntitiesIds.Contains(x.Id)));
    }

    public void AddRemoveChildValueObjects<TDependent>(IEnumerable<TDependent> actualEntities,
        IEnumerable<TDependent> dbEntities)
        where TDependent : ValueObject
    {
        var addedEntities = actualEntities.Except(dbEntities);
        Set<TDependent>().AddRange(addedEntities);
    
        var removedEntities = dbEntities.Except(actualEntities);
        foreach(var removedEntity in removedEntities)
        {
            var existingEntity = Set<TDependent>().Local
                .FirstOrDefault(x => x == removedEntity);

            if (existingEntity is not null)
            {
                Entry(existingEntity).State = EntityState.Detached;
            }

            Set<TDependent>().Attach(removedEntity);
            Set<TDependent>().Remove(removedEntity);
        }
    }

    private async System.Threading.Tasks.Task DispatchDomainEvents(CancellationToken cancellationToken)
    {
        // TODO: Later move to outbox pattern
        var entities = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.Events.Any())
            .Select(e => e.Entity)
            .ToList();

        var events = entities
            .SelectMany(e => e.Events)
            .ToList();

        entities.ForEach(e => e.ClearEvents());

        foreach (var @event in events)
        {
            await domainEventDispatcher.Dispatch(@event);
        }
    }
}
