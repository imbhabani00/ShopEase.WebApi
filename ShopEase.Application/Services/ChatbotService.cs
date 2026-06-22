using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ecommerce.Application.Services
{
    public interface IChatbotService
    {
        Task<string> GetResponseAsync(string userMessage, string conversationHistory = "");
    }
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public ChatbotService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ChatbotSettings:ApiKey"];
            _model = configuration["ChatbotSettings:Model"] ?? "gpt-3.5-turbo";

            _httpClient.BaseAddress = new Uri("https://api.openai.com/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetResponseAsync(string userMessage, string conversationHistory = "")
        {
            var messages = new List<object>
            {
                new { role = "system", content = "You are a helpful ecommerce assistant. Help users with product queries, order tracking, and general shopping assistance." }
            };

            // Include conversation history if provided
            if (!string.IsNullOrEmpty(conversationHistory))
            {
                messages.Add(new { role = "assistant", content = conversationHistory });
            }

            messages.Add(new { role = "user", content = userMessage });

            var requestBody = new
            {
                model = _model,
                messages = messages,
                max_tokens = 500
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("v1/chat/completions", content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);

            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
    }
}