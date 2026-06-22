namespace Ecommerce.Application.DTOs.Request
{
    public class InitiatePaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
