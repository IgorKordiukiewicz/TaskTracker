using Flurl;
using MudBlazor;
using Shared.Dtos;
using Shared.ViewModels;

namespace Web.Client.Services;

public class WorkflowsService : ApiService
{
    public WorkflowsService(HttpClient httpClient, ISnackbar snackbar) 
        : base(httpClient, snackbar)
    {
    }

    public async Task<WorkflowVM?> Get(Guid projectId)
    {
        var url = "workflows"
            .SetQueryParam("projectId", projectId)
            .ToString();
        return await Get<WorkflowVM>(url);
    }

    public async Task<bool> AddStatus(Guid workflowId, Guid projectId, AddWorkflowStatusDto model)
        => await Post($"workflows/{workflowId}/statuses", model, ProjectIdHeader(projectId));

    public async Task<bool> AddTransition(Guid workflowId, Guid projectId, AddWorkflowTransitionDto model)
        => await Post($"workflows/{workflowId}/transitions", model, ProjectIdHeader(projectId));

    public async Task<bool> DeleteStatus(Guid workflowId, Guid statusId, Guid projectId)
        => await Post($"workflows/{workflowId}/statuses/{statusId}/delete", ProjectIdHeader(projectId));

    public async Task<bool> DeleteTransition(Guid workflowId, Guid projectId, DeleteWorkflowTransitionDto model)
        => await Post($"workflows/{workflowId}/transitions/delete", model, ProjectIdHeader(projectId));

    public async Task<bool> ChangeInitialStatus(Guid workflowId, Guid projectId, ChangeInitialWorkflowStatusDto model)
        => await Post($"workflows/{workflowId}/change-initial-status", model, ProjectIdHeader(projectId));

    private static Headers ProjectIdHeader(Guid projectId)
        => Headers.From(("ProjectId", projectId.ToString()));
}
