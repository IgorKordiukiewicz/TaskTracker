using Domain.Notifications;

namespace UnitTests.Domain;

public class NotificationTests
{
    [Fact]
    public void MarkAsRead_ShouldSetReadFlagToTrue()
    {
        var notification = Notification.FromData(new(Guid.NewGuid(), "abc", DateTime.Now, NotificationContext.Organization, Guid.NewGuid()));

        notification.MarkAsRead();

        notification.Read.Should().BeTrue();
    }
}
