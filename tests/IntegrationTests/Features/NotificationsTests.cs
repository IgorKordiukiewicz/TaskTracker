using Application.Features.Notifications;
using Domain.Notifications;

namespace IntegrationTests.Features;

[Collection(nameof(IntegrationTestsCollection))]
public class NotificationsTests
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly EntitiesFactory _factory;

    public NotificationsTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
        _factory = new(_fixture);

        _fixture.ResetDb();
    }

    [Fact]
    public async Task Create_ShouldCreateNotification()
    {
        var user = (await _factory.CreateUsers())[0];
        var notification = Notification.Create(user.Id, "", DateTime.Now, NotificationContext.Organization, Guid.NewGuid());

        await _fixture.SendRequest(new CreateNotificationCommand(notification));

        (await _fixture.CountAsync<Notification>(x => x.Id == notification.Id)).Should().Be(1);
    }
}
