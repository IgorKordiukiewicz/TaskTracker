using Infrastructure.Models;

namespace Application.Common;

public interface ITasksBoardLayoutService
{
    Task HandleChanges(Guid projectId, Action<TasksBoardLayout> action, CancellationToken cancellationToken = default);
}

public class TasksBoardLayoutService(AppDbContext dbContext)
    : ITasksBoardLayoutService
{
    public async Task HandleChanges(Guid projectId, Action<TasksBoardLayout> action, CancellationToken cancellationToken = default)
    {
        var layout = await dbContext.TasksBoardLayouts.SingleOrDefaultAsync(x => x.ProjectId == projectId, cancellationToken);
        if(layout is null)
        {
            layout = new TasksBoardLayout()
            {
                ProjectId = projectId
            };

            action(layout);
            dbContext.TasksBoardLayouts.Add(layout);
        }
        else
        {
            action(layout);
            dbContext.TasksBoardLayouts.Update(layout);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}