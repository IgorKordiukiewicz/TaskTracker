﻿using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Infrastructure.Models;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }
    public DbSet<OrganizationRole> OrganizationRoles { get; set; }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }

    public DbSet<Domain.Tasks.Task> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<TaskActivity> TaskActivities { get; set; }

    public DbSet<Domain.Workflows.TaskStatus> TaskStatuses { get; set; }
    public DbSet<TaskStatusTransition> TaskStatusTransitions { get; set; }
    public DbSet<Workflow> Workflows { get; set; }

    public DbSet<UserPresentationData> UsersPresentationData { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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
}
