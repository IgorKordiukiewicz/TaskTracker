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
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationsVM), 200)]
    public async Task<IActionResult> GetNotifications()
    {
        var result = await mediator.Send(new GetNotificationsQuery(User.GetUserId()));
        return Ok(result);
    }
}
