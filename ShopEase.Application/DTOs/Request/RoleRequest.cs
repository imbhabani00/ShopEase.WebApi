namespace ShopEase.Application.DTOs.Request
{
    public class RoleRequest
    {
        public int? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}