using Blazor.Diagrams;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Shared.ViewModels;
using Web.Client.Diagrams;

namespace Web.Client.Common;

public static class DiagramExtensions
{
    public static void InitializeStatusNodes(this BlazorDiagram diagram, 
        IReadOnlyList<WorkflowTaskStatusVM> Statuses, out Dictionary<Guid, TaskStatusNodeModel> nodeByStatusId)
    {
        var currentPositionX = 50;
        nodeByStatusId = new Dictionary<Guid, TaskStatusNodeModel>();

        foreach (var status in Statuses)
        {
            var node = diagram.Nodes.Add(new TaskStatusNodeModel(position: new Point(currentPositionX, 50))
            {
                Name = status.Name.ToUpper()
            });
            nodeByStatusId.Add(status.Id, node);

            currentPositionX += 150;
        }
    }

    public static void InitializeTransitionLinks(this BlazorDiagram diagram, 
        IReadOnlyList<WorkflowTaskStatusTransitionVM> Transitions, IReadOnlyDictionary<Guid, TaskStatusNodeModel> nodeByStatusId)
    {
        var linkCreatedByTransitionId = Transitions.ToDictionary(k => (k.FromStatusId, k.ToStatusId), v => false);
        foreach (var transition in Transitions)
        {
            var transitionKey = transition.GetTransitionKey();
            if (linkCreatedByTransitionId[transitionKey])
            {
                continue;
            }

            var fromNode = nodeByStatusId[transition.FromStatusId];
            var toNode = nodeByStatusId[transition.ToStatusId];

            var reverseTransitionKey = transition.GetReverseTransitionKey();
            var isBidirectional = linkCreatedByTransitionId.ContainsKey(reverseTransitionKey);

            diagram.Links.Add(DiagramFactory.CreateLink(fromNode, toNode, isBidirectional));

            linkCreatedByTransitionId[transitionKey] = true;
            if (isBidirectional)
            {
                linkCreatedByTransitionId[reverseTransitionKey] = true;
            }
        }
    }

    public static void AddStatusNode(this BlazorDiagram diagram, string newStatus)
    {
        diagram.Nodes.Add(new TaskStatusNodeModel(position: new Point(50, 50))
        {
            Name = newStatus.ToUpper()
        });
    }

    public static void DeleteStatusNode(this BlazorDiagram diagram, string deletedStatus)
    {
        var deletedStatusNameUpper = deletedStatus.ToUpper();
        var node = diagram.Nodes.First(x => (x as TaskStatusNodeModel)!.Name == deletedStatusNameUpper);
        diagram.Nodes.Remove(node);

        var relatedLinks = diagram.Links.Where(x =>
            (x.Source.Model as TaskStatusNodeModel)!.Name == deletedStatusNameUpper
            || (x.Target.Model as TaskStatusNodeModel)!.Name == deletedStatusNameUpper);
        diagram.Links.Remove(relatedLinks);
    }

    public static void AddTransitionLink(this BlazorDiagram diagram, (string FromStatus, string ToStatus) newTransition)
    {
        var nodeModelByStatusName = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!)
                .ToDictionary(k => k.Name, v => v, StringComparer.OrdinalIgnoreCase);

        var fromStatusNode = nodeModelByStatusName[newTransition.FromStatus];
        var toStatusNode = nodeModelByStatusName[newTransition.ToStatus];

        var link = diagram.Links.FirstOrDefault(x => (x.Source.Model == fromStatusNode && x.Target.Model == toStatusNode)
            || (x.Source.Model == toStatusNode && x.Target.Model == fromStatusNode));
        if (link is not null)
        {
            // Set the currently null marker to arrow
            link.SourceMarker ??= LinkMarker.Arrow;
            link.TargetMarker ??= LinkMarker.Arrow;

            link.Refresh();
        }
        else
        {
            diagram.Links.Add(DiagramFactory.CreateLink(fromStatusNode, toStatusNode));
        }
    }

    public static void DeleteTransitionLink(this BlazorDiagram diagram, (string FromStatus, string ToStatus) deletedTransition)
    {
        var nodeModelByStatusName = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!)
                .ToDictionary(k => k.Name, v => v, StringComparer.OrdinalIgnoreCase);

        var fromStatusNode = nodeModelByStatusName[deletedTransition.FromStatus];
        var toStatusNode = nodeModelByStatusName[deletedTransition.ToStatus];

        var link = diagram.Links.First(x => (x.Source.Model == fromStatusNode && x.Target.Model == toStatusNode)
            || (x.Source.Model == toStatusNode && x.Target.Model == fromStatusNode));
        if (link.IsBidirectional())
        {
            if(link.Source.Model == fromStatusNode)
            {
                link.TargetMarker = null;
            }
            else
            {
                link.SourceMarker = null;
            }
                
            link.Refresh();
        }
        else
        {
            diagram.Links.Remove(link);
        }
    }

    public static bool IsBidirectional(this BaseLinkModel link)
        => link.TargetMarker == LinkMarker.Arrow && link.SourceMarker == LinkMarker.Arrow;

    private static (Guid, Guid) GetTransitionKey(this WorkflowTaskStatusTransitionVM transitionVM)
        => (transitionVM.FromStatusId, transitionVM.ToStatusId);

    private static (Guid, Guid) GetReverseTransitionKey(this WorkflowTaskStatusTransitionVM transitionVM)
        => (transitionVM.ToStatusId, transitionVM.FromStatusId);
}
