using Domain.Users;

namespace UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUserWithGivenParameters()
    {
        var authenticationId = "12345";
        var email = "UserName";
        var firstName = "FirstName";
        var lastName = "LastName";

        var user = User.Create(authenticationId, email, firstName, lastName);

        using (new AssertionScope())
        {
            user.AuthenticationId.Should().Be(authenticationId);
            user.Email.Should().Be(email);
            user.Id.Should().NotBeEmpty();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }
    }
}
