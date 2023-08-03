using Domain.Users;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class TestTest : IClassFixture<IntegrationTestsFixture>
{
    private readonly IntegrationTestsFixture _fixture;

    public TestTest(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test()
    {
        var x = _fixture.GetAsync<User>();
    }
}
