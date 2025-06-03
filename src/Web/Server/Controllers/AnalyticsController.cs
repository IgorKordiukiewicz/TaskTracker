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
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TotalTaskStatusesVM), 200)]
    public async Task<IActionResult> GetTotalTaskStatuses(Guid projectId)
    {
        return Ok(await queryService.GetTotalTaskStatuses(projectId));
    }

    /// <summary>
    /// Get total count of each status for each day in the project history.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet("{projectId:guid}/tasks/daily-statuses")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TotalTaskStatusesByDayVM), 200)]
    public async Task<IActionResult> GetTotalTaskStatusesByDay(Guid projectId)
    {
        return Ok(await queryService.GetTotalTaskStatusesByDay(projectId));
    }

    /// <summary>
    /// Get total count of each priority for current day.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet("{projectId:guid}/tasks/priorities")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TotalTaskPrioritiesVM), 200)]
    public async Task<IActionResult> GetTotalTaskPriorities(Guid projectId)
    {
        return Ok(await queryService.GetTotalTaskPriorites(projectId));
    }

    /// <summary>
    /// Get total count of each priority for each day in the project history.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet("{projectId:guid}/tasks/daily-priorities")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TotalTaskPrioritiesByDayVM), 200)]
    public async Task<IActionResult> GetTotalTaskPrioritiesByDay(Guid projectId)
    {
        return Ok(await queryService.GetTotalTaskPrioritiesByDay(projectId));
    }
}
