namespace Ecommerce.Application.DTOs.Request
{
    public class RefundRequest
    {
        public string GatewayPaymentId { get; set; }
        public decimal Amount { get; set; }
        public int OrderId { get; set; }
    }
}