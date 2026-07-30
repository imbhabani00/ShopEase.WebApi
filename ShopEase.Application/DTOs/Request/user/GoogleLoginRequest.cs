namespace ShopEase.Application.DTOs.Request.user
{
    public class GoogleLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }
}
