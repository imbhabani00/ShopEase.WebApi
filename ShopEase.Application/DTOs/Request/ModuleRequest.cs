namespace ShopEase.Application.DTOs.Request
{
    public class ModuleRequest
    {
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}