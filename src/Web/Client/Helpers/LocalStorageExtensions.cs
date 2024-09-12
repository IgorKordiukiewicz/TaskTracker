using Blazor.Diagrams;
using Blazored.LocalStorage;
using Web.Client.Diagrams;

namespace Web.Client.Common;

public class WorkflowDiagramLayout
{
    public required IReadOnlyDictionary<string, WorkflowDiagramPosition> PositionByNodeName { get; init; }
}

public record WorkflowDiagramPosition(double X, double Y);

public static class LocalStorageExtensions
{
    private static string GetWorkflowDiagramLayoutKey(Guid projectId) => $"workflow:{projectId}";

    public static async Task SaveDiagramLayout(this ILocalStorageService storageService, BlazorDiagram diagram, Guid projectId)
    {
        var positionByNodeName = new Dictionary<string, WorkflowDiagramPosition>();
        foreach(var node in diagram.Nodes.Cast<TaskStatusNodeModel>())
        {
            positionByNodeName.Add(node.Name, new(node.Position.X, node.Position.Y));
        }
        await storageService.SetItemAsync(GetWorkflowDiagramLayoutKey(projectId), 
            new WorkflowDiagramLayout() { PositionByNodeName = positionByNodeName });
    }

    public static async Task<WorkflowDiagramLayout?> GetDiagramLayout(this ILocalStorageService storageService, Guid projectId)
    {
        var key = GetWorkflowDiagramLayoutKey(projectId);
        if (await storageService.ContainKeyAsync(key))
        {
            return await storageService.GetItemAsync<WorkflowDiagramLayout>(GetWorkflowDiagramLayoutKey(projectId));
        }

        return null;
    }
}
