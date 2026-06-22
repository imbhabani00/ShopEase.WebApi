using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ecommerce.Application.Services
{
    public interface IPaymentGatewayService
    {
        Task<CreateOrderResult> CreatePaymentOrderAsync(decimal amount, string currency, string receiptId);
        Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature);
        Task<RefundResult> InitiateRefundAsync(string paymentId, decimal amount);
        Task<PaymentDetailsResult> GetPaymentDetailsAsync(string paymentId);
    }
    public class RazorpayService
    {
        private readonly HttpClient _httpClient;
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _keyId = configuration["PaymentSettings:RazorpayKeyId"];
            _keySecret = configuration["PaymentSettings:RazorpayKeySecret"];

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_keyId}:{_keySecret}"));
            _httpClient.BaseAddress = new Uri("https://api.razorpay.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);
        }

        #region CreatePaymentOrder
        public async Task<CreateOrderResult> CreatePaymentOrderAsync(decimal amount, string currency, string receiptId)
        {
            var requestBody = new
            {
                amount = (int)(amount * 100), // Razorpay accepts paise
                currency = currency,
                receipt = receiptId
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("orders", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            return new CreateOrderResult
            {
                OrderId = doc.RootElement.GetProperty("id").GetString(),
                Amount = amount,
                Currency = currency,
                Status = doc.RootElement.GetProperty("status").GetString()
            };
        }
        #endregion

        #region VerifyPaymentSignature
        public async Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature)
        {
            var payload = $"{orderId}|{paymentId}";

            using var hmac = new System.Security.Cryptography.HMACSHA256(
                Encoding.UTF8.GetBytes(_keySecret));

            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSig = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return await Task.FromResult(generatedSig == signature);
        }
        #endregion

        #region InitiateRefund
        public async Task<RefundResult> InitiateRefundAsync(string paymentId, decimal amount)
        {
            var requestBody = new
            {
                amount = (int)(amount * 100) // paise
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"payments/{paymentId}/refund", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            return new RefundResult
            {
                RefundId = doc.RootElement.GetProperty("id").GetString(),
                PaymentId = paymentId,
                Amount = amount,
                Status = doc.RootElement.GetProperty("status").GetString()
            };
        }
        #endregion

        #region GetPaymentDetails
        public async Task<PaymentDetailsResult> GetPaymentDetailsAsync(string paymentId)
        {
            var response = await _httpClient.GetAsync($"payments/{paymentId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            return new PaymentDetailsResult
            {
                PaymentId = paymentId,
                Amount = doc.RootElement.GetProperty("amount").GetDecimal() / 100,
                Currency = doc.RootElement.GetProperty("currency").GetString(),
                Status = doc.RootElement.GetProperty("status").GetString(),
                Method = doc.RootElement.GetProperty("method").GetString()
            };
        }
        #endregion
    }

    // ── Result Models ─────────────────────────────────
    public class CreateOrderResult
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }

    public class RefundResult
    {
        public string RefundId { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class PaymentDetailsResult
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
    }
}