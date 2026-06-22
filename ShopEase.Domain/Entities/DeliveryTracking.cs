using Ecommerce.Application.DTOs.Domain.Base;

namespace Ecommerce.Domain.Entities
{
    public class DeliveryTracking : BaseEntity
    {
        public int OrderId { get; set; }
        public int DeliveryAgentId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FormattedAddress { get; set; }
        public string Status { get; set; } 
        public DateTime TrackedAt { get; set; } = DateTime.UtcNow;
    }
}