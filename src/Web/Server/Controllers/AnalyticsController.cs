using Analytics.Services;
using Analytics.ViewModels;

namespace Web.Server.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="queryService"></param>
[ApiController]
[Route("analytics")]
[Authorize]
public class AnalyticsController(IQueryService queryService)
    : ControllerBase
{
    /// <summary>
    /// Get total count of each status for current day.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet("{projectId:guid}/tasks/statuses")]
    [ProducesResponseType(typeof(TotalTaskStatusesVM), 200)]
    public async Task<IActionResult> GetTotalTaskStatuses(Guid projectId)
    {
        return Ok(await queryService.GetTotalTaskStatuses(projectId));
    }
}
