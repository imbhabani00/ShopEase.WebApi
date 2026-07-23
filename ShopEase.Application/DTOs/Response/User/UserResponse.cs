namespace Ecommerce.Application.DTOs.Response.User
{
    #region UserResponse
    public class UserResponse
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
    #endregion

    #region UserGetResponse
    public class UserGetResponse
    {
        public UserResponse? User { get; set; }
        public int? ReturnValue { get; set; }
    }
    #endregion

    #region UsersResponse
    public class UsersResponse
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    #endregion

    #region UsersResponseList
    public class UsersResponseList
    {
        public List<UsersResponse> UsersData { get; set; }
        public int TotalCount { get; set; }
        public int ReturnValue { get; set; }
    }
    #endregion
}