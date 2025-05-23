using Application.Features.Notifications;
using Domain.Notifications;
using Domain.Users;

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
        var notificationData = new NotificationData(user.Id, "", DateTime.Now, Guid.NewGuid());

        await _fixture.SendRequest(new CreateNotificationCommand(notificationData));

        (await _fixture.CountAsync<Notification>()).Should().Be(1);
    }

    [Fact]
    public async Task Read_ShouldFail_WhenNotificationDoesNotExist()
    {
        var result = await _fixture.SendRequest(new ReadNotificationCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Read_ShouldFail_WhenItDoesNotBelongToCurrentUser()
    {
        var user = (await _factory.CreateUsers())[0];
        var notification = Notification.FromData(new(user.Id, "abc", DateTime.Now, Guid.NewGuid()));

        await _fixture.SeedDb(db =>
        {
            db.Add(notification);
        });

        var result = await _fixture.SendRequest(new ReadNotificationCommand(notification.Id, Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Read_ShouldSucceed_WhenNotificationExistsAndBelongsToUser()
    {
        var user = (await _factory.CreateUsers())[0];
        var notification = Notification.FromData(new(user.Id, "abc", DateTime.Now, Guid.NewGuid()));

        await _fixture.SeedDb(db =>
        {
            db.Add(notification);
        });

        var result = await _fixture.SendRequest(new ReadNotificationCommand(notification.Id, user.Id));

        result.IsSuccess.Should().BeTrue();
    }
}
