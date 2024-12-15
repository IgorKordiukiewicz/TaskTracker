using Application.Features.Notifications;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="mediator"></param>
[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController(IMediator mediator)
    : ControllerBase
{
    /// <summary>
    /// Get all of user's unread notifications.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationsVM), 200)]
    public async Task<IActionResult> GetNotifications()
    {
        var result = await mediator.Send(new GetNotificationsQuery(User.GetUserId()));
        return Ok(result);
    }

    /// <summary>
    /// Mark a given notification as read.
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ReadNotification(Guid id)
    {
        var result = await mediator.Send(new ReadNotificationCommand(id, User.GetUserId()));
        return result.ToHttpResult();
    }
}
