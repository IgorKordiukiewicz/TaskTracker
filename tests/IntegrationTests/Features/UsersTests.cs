using Application.Features.Users;
using Domain.Users;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class UsersTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public UsersTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task UpdateUserName_ShouldUpdateFirstAndLastName_WhenUserWithGivenAuthIdExists()
    {
        var user = (await _factory.CreateUsers())[0];
        var newFirstName = user.FirstName + "A";
        var newLastName = user.LastName + "A";

        var result = await _fixture.SendRequest(new UpdateUserNameCommand(user.Id, new(newFirstName, newLastName)));

        using(new AssertionScope()) 
        {
            result.IsSuccess.Should().BeTrue();
            var updatedUser = await _fixture.FirstAsync<User>(x => x.Id == user.Id);
            updatedUser.FirstName.Should().Be(newFirstName);
            updatedUser.LastName.Should().Be(newLastName);
        }
    }
}
