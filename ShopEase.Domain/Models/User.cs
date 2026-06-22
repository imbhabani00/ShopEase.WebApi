namespace Ecommerce.Domain.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public int? TenantId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? DeletedBy { get; set; }
        public string? PasswordChangedDate { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PhoneNumber { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

    public class UserGet
    {
        public User? User { get; set; }
        public int ReturnValue { get; set; }
    }
}