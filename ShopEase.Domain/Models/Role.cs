namespace ShopEase.Domain.Models
{
    public class Role : SortWithPageParameters
    {
        public int RoleId { get; set; }
        public int TenantId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class RoleList
    {
        public List<Role> Roles { get; set; }
        public int TotalCount { get; set; }
    }
}  