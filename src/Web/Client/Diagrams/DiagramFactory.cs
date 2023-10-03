using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;

namespace Web.Client.Diagrams;

public static class DiagramFactory // TODO: Remove, diagram options have factory parameters?
{
    public static LinkModel CreateLink(TaskStatusNodeModel sourceNode, TaskStatusNodeModel targetNode)
        => new(sourceNode, targetNode)
        {
            TargetMarker = LinkMarker.Arrow,
        };
}
