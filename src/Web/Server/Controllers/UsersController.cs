using Application.Features.Users;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("users")]
[Authorize]
[ProducesResponseType(400)]
[ProducesResponseType(500)]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Check whether user has been already registered in the system.
    /// </summary>
    [HttpGet("me/registered")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> IsUserRegistered()
    {
        var result = await _mediator.Send(new IsUserRegisteredQuery(User.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get user's data along with their permissions.
    /// </summary>
    /// <param name="userId"></param>
    /// <response code="404">User not found.</response> 
    [HttpGet("{userId}/data")] // without /data the endpoint is not called
    [ProducesResponseType(typeof(UserVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var result = await _mediator.Send(new GetUserQuery(userId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Register user in the system.
    /// </summary>
    [HttpPost("me/register")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto model) 
    {
        var result = await _mediator.Send(new RegisterUserCommand(User.GetUserId(), model));
        return result.ToHttpResult(201);
    }

    /// <summary>
    /// Get users that are available for organization invitation.
    /// </summary>
    /// <param name="searchValue"></param>
    /// <param name="organizationId"></param>
    /// <response code="404">Organization not found.</response> 
    [HttpGet("available-for-invitation")]
    [Authorize(Policy.OrganizationInviteMembers)]
    [ProducesResponseType(typeof(UsersSearchVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUsersNotInOrganization([FromQuery] string searchValue, [FromQuery] Guid organizationId)
    {
        var result = await _mediator.Send(new GetUsersAvailableForOrganizationInvitationQuery(organizationId, searchValue));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get organization members that are available to be added to the given project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="organizationId"></param>
    /// <response code="404">Project or organization not found.</response> 
    [HttpGet("available-for-project")]
    [Authorize(Policy.ProjectAddMembers)]
    [ProducesResponseType(typeof(UsersSearchVM), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUsersAvailableForProject([FromQuery] Guid projectId, [FromQuery] Guid organizationId)
    {
        var result = await _mediator.Send(new GetUsersAvailableForProjectQuery(organizationId, projectId));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Update a user's first and last name.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <response code="404">User not found.</response> 
    [HttpPost("{userId:guid}/update-name")]
    [Authorize(Policy.UserSelf)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateUserName(Guid userId, [FromBody] UpdateUserNameDto model)
    {
        var result = await _mediator.Send(new UpdateUserNameCommand(userId, model));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Get presentation data of all users.
    /// </summary>
    /// <remarks>
    /// Only returns data of users that the current user can see.
    /// </remarks>
    /// <response code="404">User not found.</response> 
    [HttpGet("presentation")]
    [Authorize]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAllUsersPresentationData()
    {
        var result = await _mediator.Send(new GetAllUsersPresentationDataQuery(User.GetUserId()));
        return result.ToHttpResult();
    }
}
