using ShopEase.Domain.Models;

namespace ShopEase.Application.DTOs.Response.Role
{
    public class RoleResponse :SortWithPageParameters
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class RoleResponseList
    {
        public List<RoleResponse> Roles { get; set; }
        public int TotalCount { get; set; }
    }
}