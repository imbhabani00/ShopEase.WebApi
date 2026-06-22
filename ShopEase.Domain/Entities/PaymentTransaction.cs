using Ecommerce.Application.DTOs.Domain.Base;

namespace Ecommerce.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string GatewayOrderId { get; set; }      // Razorpay order id
        public string GatewayPaymentId { get; set; }    // Razorpay payment id
        public string GatewaySignature { get; set; }    // for verification
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMethod { get; set; }       // "upi", "card", "netbanking"
        public string Status { get; set; }              // "Pending","Captured","Failed","Refunded"
        public string? FailureReason { get; set; }
        public string? RefundId { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}