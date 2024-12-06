using Application.Features.Notifications;
using Domain.Notifications;
using Domain.Organizations;
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
        var notificationData = new NotificationData(user.Id, "", DateTime.Now, NotificationContext.Organization, Guid.NewGuid());

        await _fixture.SendRequest(new CreateNotificationCommand(notificationData));

        (await _fixture.CountAsync<Notification>()).Should().Be(1);
    }

    [Fact]
    public async Task Get_ShouldReturnAllUnreadNotificationsForUserWithMatchedEntityNames()
    {
        var project = (await _factory.CreateProjects())[0];
        var user = await _fixture.FirstAsync<User>();
        var organization = await _fixture.FirstAsync<Organization>();

        var user2 = User.Create(Guid.NewGuid(), "bb", "bb", "bb");

        var now = DateTime.Now;

        var notification1 = Notification.FromData(new(user.Id, "abc", now, NotificationContext.Organization, organization.Id));
        var notification2 = Notification.FromData(new(user.Id, "abc", now.AddDays(1), NotificationContext.Project, project.Id));
        var notification3 = Notification.FromData(new(user2.Id, "abc", now, NotificationContext.Project, project.Id));

        await _fixture.SeedDb(db =>
        {
            db.Add(user2);
            db.AddRange(notification1, notification2, notification3);
        });

        var result = await _fixture.SendRequest(new GetNotificationsQuery(user.Id));

        using(new AssertionScope())
        {
            result.Notifications.Should().HaveCount(2);
            result.Notifications[0].Id.Should().Be(notification2.Id);
            result.Notifications[0].ContextEntityName.Should().Be(project.Name);
            result.Notifications[1].Id.Should().Be(notification1.Id);
            result.Notifications[1].ContextEntityName.Should().Be(organization.Name);
        }
    }
}
