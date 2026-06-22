using System.Security.Claims;

namespace Ecommerce.Api.Helper
{
    public static class ClaimsHelper
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }

        public static string? GetTenantId(this ClaimsPrincipal user)
        {
            return user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email)?.Value;

        public static string? GetUserRole(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Role)?.Value;

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole("Admin");

        public static bool IsSeller(this ClaimsPrincipal user)
            => user.IsInRole("Seller");

        public static bool IsBuyer(this ClaimsPrincipal user)
            => user.IsInRole("Buyer");
    }
}