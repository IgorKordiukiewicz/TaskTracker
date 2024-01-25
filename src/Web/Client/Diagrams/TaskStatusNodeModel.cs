using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace Web.Client.Diagrams;

public class TaskStatusNodeModel : NodeModel
{
    public TaskStatusNodeModel(Point position)
        : base(position)
    {
        
    }

    public required string Name { get; set; }
    public bool Initial { get; set; } = false;
}
