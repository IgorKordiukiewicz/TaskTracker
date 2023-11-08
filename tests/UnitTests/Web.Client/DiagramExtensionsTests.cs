using Blazor.Diagrams;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Shared.ViewModels;
using Web.Client.Common;
using Web.Client.Diagrams;

namespace UnitTests.Web.Client;

public class DiagramExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void InitializeStatusNodes_ShouldAddAllStatusesAsNodesAndInitializeNodeByStatusId()
    {
        var diagram = new BlazorDiagram();
        var statuses = _fixture.CreateMany<WorkflowTaskStatusVM>(4).ToList();

        diagram.InitializeStatusNodes(statuses, null, out var nodeByStatusId);

        using(new AssertionScope())
        {
            var nodesNames = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!.Name);
            nodesNames.Should().BeEquivalentTo(statuses.Select(x => x.Name.ToUpper()));

            var statusIdByName = statuses.ToDictionary(k => k.Name, v => v.Id, StringComparer.OrdinalIgnoreCase);
            var expectedNodeByStatusId = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!).ToDictionary(k => statusIdByName[k.Name], v => v);
            nodeByStatusId.Should().BeEquivalentTo(expectedNodeByStatusId);
        }
    }

    [Fact]
    public void InitializeTransitionLinks_ShouldAddAllTransitionsAsLinks()
    {
        var statusesIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var nodeByStatusId = new Dictionary<Guid, TaskStatusNodeModel>();
        foreach(var statusId in statusesIds)
        {
            var nodeModel = new TaskStatusNodeModel(new(0.0, 0.0))
            {
                Name = statusId.ToString(),
            };
            nodeByStatusId.Add(statusId, nodeModel);
        }

        var transitions = new List<WorkflowTaskStatusTransitionVM>()
        {
            new(statusesIds[0], statusesIds[1]),
            new(statusesIds[1], statusesIds[0]),
            new(statusesIds[0], statusesIds[2]),
            new(statusesIds[3], statusesIds[2]),
        };

        var diagram = new BlazorDiagram();

        diagram.InitializeTransitionLinks(transitions, nodeByStatusId);

        using(new AssertionScope())
        {
            // Select into array instead of tuple to ignore order
            var linksIds = diagram.Links.Select(x => 
                new[] { Guid.Parse((x.Source.Model as TaskStatusNodeModel)!.Name), Guid.Parse((x.Target.Model as TaskStatusNodeModel)!.Name) });
            linksIds.Should().BeEquivalentTo(new[]
            {
                new[] { statusesIds[0], statusesIds[1] },
                new[] { statusesIds[0], statusesIds[2] },
                new[] { statusesIds[2], statusesIds[3] },
            });
        }
    }

    [Fact]
    public void AddStatusNode_ShouldAddNewNode()
    {
        var diagram = new BlazorDiagram();

        diagram.AddStatusNode("abc");

        var nodesNames = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!.Name.ToLower());
        nodesNames.Should().BeEquivalentTo(new[] { "abc" });
    }

    [Fact]
    public void DeleteStatusNode_ShouldDeleteNodeAndItsRelatedLinks()
    {
        var diagram = new BlazorDiagram();
        var node1 = AddNode(diagram, "111");
        var node2 = AddNode(diagram, "222");
        var node3 = AddNode(diagram, "333");
        AddLink(diagram, node1, node2);
        AddLink(diagram, node3, node1);

        diagram.DeleteStatusNode(node1.Name);

        using(new AssertionScope())
        {
            var nodesNames = diagram.Nodes.Select(x => (x as TaskStatusNodeModel)!.Name.ToLower());
            nodesNames.Should().BeEquivalentTo(new[] { node2.Name, node3.Name });

            diagram.Links.Should().BeEmpty();
        }
    }

    [Fact]  
    public void AddTransitionLink_ShouldAddNewLink_WhenNewTransitionIsNotBidirectional()
    {
        var diagram = new BlazorDiagram();
        var node1 = AddNode(diagram, "111");
        var node2 = AddNode(diagram, "222");

        diagram.AddTransitionLink((node1.Name, node2.Name));

        diagram.Links.Count.Should().Be(1);
    }

    [Fact] 
    public void AddTransitionLink_ShouldUpdateExistingLink_WhenNewTransitionIsBidirectional()
    {
        var diagram = new BlazorDiagram();
        var node1 = AddNode(diagram, "111");
        var node2 = AddNode(diagram, "222");
        AddLink(diagram, node1, node2);

        diagram.AddTransitionLink((node2.Name, node1.Name));

        using(new AssertionScope())
        {
            diagram.Links.Count.Should().Be(1);
            var link = diagram.Links[0];
            link.SourceMarker.Should().Be(LinkMarker.Arrow);
            link.TargetMarker.Should().Be(LinkMarker.Arrow);
        }
    }

    [Fact]
    public void DeleteTransitionLink_ShouldDeleteLink_WhenDeletedTransitionIsNotBidirectional()
    {
        var diagram = new BlazorDiagram();
        var node1 = AddNode(diagram, "111");
        var node2 = AddNode(diagram, "222");
        AddLink(diagram, node1, node2);

        diagram.DeleteTransitionLink((node1.Name, node2.Name));

        diagram.Links.Should().BeEmpty();
    }

    [Fact]
    public void DeleteTransitionLink_ShouldUpdateExistingLink_WhenDeletedTransitionIsBidirectional()
    {
        var diagram = new BlazorDiagram();
        var node1 = AddNode(diagram, "111");
        var node2 = AddNode(diagram, "222");
        AddLink(diagram, node1, node2, true);

        diagram.DeleteTransitionLink((node1.Name, node2.Name));

        using(new AssertionScope())
        {
            diagram.Links.Count.Should().Be(1);
            var link = diagram.Links[0];
            link.SourceMarker.Should().Be(LinkMarker.Arrow);
            link.TargetMarker.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public void IsBidirectional_ShouldReturnCorrectValue_DependingOnLinkMarkers(bool sourceArrow, bool targetArrow, bool expected)
    {
        var link = new LinkModel(new NodeModel(), new NodeModel())
        {
            SourceMarker = sourceArrow ? LinkMarker.Arrow : null,
            TargetMarker = targetArrow ? LinkMarker.Arrow : null,
        };

        var result = link.IsBidirectional();

        result.Should().Be(expected);
    }

    private static TaskStatusNodeModel AddNode(BlazorDiagram diagram, string name)
        => diagram.Nodes.Add(new TaskStatusNodeModel(position: new Point(0.0, 0.0)) { Name = name });

    private static void AddLink(BlazorDiagram diagram, TaskStatusNodeModel fromNode, TaskStatusNodeModel toNode, bool bidirectional = false)
        => diagram.Links.Add(DiagramFactory.CreateLink(fromNode, toNode, bidirectional));
}
