namespace ShopEase.Application.DTOs.Request
{
    public class PermissionRequest
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanInactive { get; set; }
    }
}
