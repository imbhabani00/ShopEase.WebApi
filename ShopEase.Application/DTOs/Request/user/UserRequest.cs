namespace ShopEase.Application.DTOs.Request.user
{
    public class UserRequest
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
    }
}