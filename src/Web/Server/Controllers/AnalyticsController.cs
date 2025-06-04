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
    /// Get task analytics for a given project.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet("{projectId:guid}/tasks")]
    [Authorize(Policy.ProjectMember)]
    [ProducesResponseType(typeof(TaskAnalyticsVM), 200)]
    public async Task<IActionResult> GetTaskAnalytics(Guid projectId)
    {
        return Ok(await queryService.GetTaskAnalytics(projectId));
    }
}
