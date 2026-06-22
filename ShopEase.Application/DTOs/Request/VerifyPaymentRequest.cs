namespace Ecommerce.Application.DTOs.Request
{
    public class VerifyPaymentRequest
    {
        public string GatewayOrderId { get; set; }
        public string GatewayPaymentId { get; set; }
        public string GatewaySignature { get; set; }
        public int OrderId { get; set; }
    }
}
