using Domain.Users;

namespace UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUserWithGivenParameters()
    {
        var email = "UserName";
        var firstName = "FirstName";
        var lastName = "LastName";

        var user = User.Create(Guid.NewGuid(), email, firstName, lastName);

        using (new AssertionScope())
        {
            user.Email.Should().Be(email);
            user.Id.Should().NotBeEmpty();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }
    }

    [Fact]
    public void FullName_ShouldReturnConcatenetedFirstAndLastNames()
    {
        var firstName = "FirstName";
        var lastName = "LastName";

        var user = User.Create(Guid.NewGuid(), "email", firstName, lastName);

        user.FullName.Should().Be($"{firstName} {lastName}");
    }

    [Fact]
    public void UpdateName_ShouldUpdateBothFirstAndLastNames()
    {
        var user = User.Create(Guid.NewGuid(), "email", "first", "last");
        var newFirstName = user.FirstName + "A";
        var newLastName = user.LastName + "A";

        user.UpdateName(newFirstName, newLastName);

        using(new AssertionScope())
        {
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
        }
    }
}
