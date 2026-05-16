using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Zent.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public Guid? GetUserId()
        {
            var value =
                user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            return Guid.TryParse(value, out var id) ? id : null;
        }
        
        public Guid GetRequiredUserId()
        {
            var userId = user.GetUserId();
            // TODO: Change to custom exception
            return userId ?? throw new UnauthorizedAccessException("User identifier claim is missing or invalid.");
        }
    }
}