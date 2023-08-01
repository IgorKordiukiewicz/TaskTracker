using Domain.Users;

namespace UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUserWithGivenParameters()
    {
        var authenticationId = "12345";
        var name = "UserName";

        var user = User.Create(authenticationId, name);

        using (new AssertionScope())
        {
            user.AuthenticationId.Should().Be(authenticationId);
            user.Name.Should().Be(name);
            user.Id.Should().NotBeEmpty();
        }
    }
}
