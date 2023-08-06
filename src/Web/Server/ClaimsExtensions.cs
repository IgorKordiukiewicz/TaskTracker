using System.Security.Claims;

namespace Web.Server;

public static class ClaimsExtensions
{
    public static string GetUserAuthenticationId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
}
