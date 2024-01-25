using Blazor.Diagrams;
using Blazored.LocalStorage;
using System.Text.Json;
using Web.Client.Common;
using Web.Client.Diagrams;

namespace UnitTests.Web.Client;

public class FakeLocalStorageService : ILocalStorageService
{
    public event EventHandler<ChangingEventArgs> Changing;
    public event EventHandler<ChangedEventArgs> Changed;

    private readonly Dictionary<string, string> _items = new();

    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
        => _items.ContainsKey(key);

    public async ValueTask<string> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
        => _items[key];

    public async ValueTask<T> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
        => JsonSerializer.Deserialize<T>(_items[key]);

    public ValueTask<string> KeyAsync(int index, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
    {
        _items.Add(key, data);
    }

    public async ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        _items.Add(key, JsonSerializer.Serialize(data));
    }
}

public class LocalStorageExtensionsTests
{
    private readonly FakeLocalStorageService _localStorage = new();

    [Fact]
    public async Task GetDiagramLayout_ShouldReturnNull_WhenNoLayoutWasSaved()
    {
        var projectId = Guid.NewGuid();

        var result = await _localStorage.GetDiagramLayout(projectId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDiagramLayout_ShouldReturnNull_WhenLayoutForGivenProjectWasNotSaved()
    {
        var diagram = CreateDiagram();
        var projectId = Guid.NewGuid();

        await _localStorage.SaveDiagramLayout(diagram, Guid.NewGuid());
        var result = await _localStorage.GetDiagramLayout(projectId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDiagramLayout_ShouldReturnCorrectLayout_WhenLayoutForGivenProjectWasSaved()
    {
        var diagram = CreateDiagram();
        var projectId = Guid.NewGuid();

        await _localStorage.SaveDiagramLayout(diagram, projectId);
        var result = await _localStorage.GetDiagramLayout(projectId);

        var expectedResult = new WorkflowDiagramLayout()
        {
            PositionByNodeName = diagram.Nodes
                .Select(x => (x as TaskStatusNodeModel)!)
                .ToDictionary(k => k.Name, v => new WorkflowDiagramPosition(v.Position.X, v.Position.Y))
        };

        using(new AssertionScope())
        {
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    private static BlazorDiagram CreateDiagram()
    {
        var diagram = new BlazorDiagram();
        diagram.Nodes.Add(new TaskStatusNodeModel(new(50, 100)) { Name = "abc" });
        diagram.Nodes.Add(new TaskStatusNodeModel(new(250, 250)) { Name = "xyz" });
        diagram.Nodes.Add(new TaskStatusNodeModel(new(140, 70)) { Name = "def" });
        return diagram;
    }
}
