namespace ShopEase.Application.DTOs.Request.user
{
    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
    }
}
