using System.Security.Claims;

namespace Ecommerce.Api.Helper
{
    public static class ClaimsHelper
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;
            return null;
        }

        public static int GetTenantId(this ClaimsPrincipal user)
        {
            var tenantId = user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
            return int.TryParse(tenantId, out var id) ? id : 0;
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