using System.Security.Claims;

namespace WebApplication1.Extentions
{
    public static class ClaimPrincipalExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
