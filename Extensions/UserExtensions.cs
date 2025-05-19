using System.Security.Claims;

namespace Survey_Basket.Extensions
{
    public static class UserExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)=>
            user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
