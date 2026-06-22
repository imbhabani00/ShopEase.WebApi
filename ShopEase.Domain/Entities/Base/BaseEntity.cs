namespace Ecommerce.Application.DTOs.Domain.Base
{
    public class BaseEntity
    {
        public string CreatedBy { get; set; }
        public String CreatedDate { get; set; }
        public String ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
    }
}
