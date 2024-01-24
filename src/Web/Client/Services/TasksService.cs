using MudBlazor;
using Shared.Dtos;
using Shared.ViewModels;

namespace Web.Client.Services;

public class TasksService : ApiService
{
    public TasksService(HttpClient httpClient, ISnackbar snackbar) : base(httpClient, snackbar)
    {
    }

    public async Task<TasksVM?> GetList(Guid projectId)
        => await Get<TasksVM>("tasks", ProjectIdHeader(projectId));

    public async Task<bool> Create(Guid projectId, CreateTaskDto model)
        => await Post("tasks", model, ProjectIdHeader(projectId));

    public async Task<TaskCommentsVM?> GetComments(Guid projectId, Guid taskId)
        => await Get<TaskCommentsVM>($"tasks/{taskId}/comments", ProjectIdHeader(projectId));

    public async Task<TaskActivitiesVM?> GetActivities(Guid projectId, Guid taskId)
        => await Get<TaskActivitiesVM>($"tasks/{taskId}/activities", ProjectIdHeader(projectId));

    public async Task<bool> UpdateStatus(Guid projectId, Guid taskId, UpdateTaskStatusDto model)
        => await Post($"tasks/{taskId}/update-status", model,  ProjectIdHeader(projectId));

    public async Task<bool> UpdatePriority(Guid projectId, Guid taskId, UpdateTaskPriorityDto model)
        => await Post($"tasks/{taskId}/update-priority", model, ProjectIdHeader(projectId));

    public async Task<bool> UpdateAssignee(Guid projectId, Guid taskId, UpdateTaskAssigneeDto model)
        => await Post($"tasks/{taskId}/update-assignee", model, ProjectIdHeader(projectId));

    public async Task<bool> UpdateDescription(Guid projectId, Guid taskId, UpdateTaskDescriptionDto model)
        => await Post($"tasks/{taskId}/update-description", model, ProjectIdHeader(projectId));

    public async Task<bool> AddComment(Guid projectId, Guid taskId, AddTaskCommentDto model)
        => await Post($"tasks/{taskId}/comments", model, ProjectIdHeader(projectId));

    private static Headers ProjectIdHeader(Guid projectId)
        => Headers.From(("ProjectId", projectId.ToString()));
}
