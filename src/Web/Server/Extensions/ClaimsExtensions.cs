using System.Security.Claims;

namespace Web.Server.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(value, out Guid id))
        {
            throw new UnauthorizedAccessException("NameIdentifier Claim is invalid.");
        }

        return id;
    }
}
