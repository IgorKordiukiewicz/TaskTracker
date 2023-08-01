using System.Security.Claims;

namespace Web.Server;

public static class ClaimsExtensions
{
    public static string GetUserAuthenticationId(this IHttpContextAccessor? contextAccessor)
        => contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
}
