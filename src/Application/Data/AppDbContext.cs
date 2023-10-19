using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Users;
using Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    public DbSet<Domain.Tasks.Task> Tasks { get; set; }
    public DbSet<Domain.Workflows.TaskStatus> TaskStatuses { get; set; }
    public DbSet<TaskStatusTransition> TaskStatusTransitions { get; set; }
    public DbSet<Workflow> Workflows { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public async System.Threading.Tasks.Task AddRemoveDependentEntities<TDependent>(IEnumerable<TDependent> actualEntities)
        where TDependent : Entity
    {
        var dbEntities = await Set<TDependent>().AsNoTracking().Select(x => x.Id).ToListAsync();

        var addedEntities = actualEntities.Where(x => !dbEntities.Any(y => y == x.Id));
        Set<TDependent>().AddRange(addedEntities);

        var removedEntitiesIds = dbEntities.Where(x => !actualEntities.Any(y => y.Id == x));
        Set<TDependent>().RemoveRange(Set<TDependent>().Where(x => removedEntitiesIds.Contains(x.Id)));
    }

    public async System.Threading.Tasks.Task AddRemoveDependentValueObjects<TDependent>(IEnumerable<TDependent> actualEntities)
        where TDependent : ValueObject
    {
        var dbEntities = await Set<TDependent>().ToListAsync();
    
        var addedEntities = actualEntities.Where(x => !dbEntities.Any(y => y == x));
        Set<TDependent>().AddRange(addedEntities);
    
        var removedEntities = dbEntities.Where(x => !actualEntities.Any(y => y == x));
        foreach(var removedEntity in removedEntities)
        {
            Set<TDependent>().Remove(removedEntity);
        }
    }
}
