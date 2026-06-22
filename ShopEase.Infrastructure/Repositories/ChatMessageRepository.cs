using Dapper;
using Ecommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Ecommerce.Infrastructure.Repositories
{
    public interface IChatMessageRepository
    {
        Task SaveMessageAsync(int userId, string sessionId, string senderType, string message);
        Task<List<ChatMessage>> GetBySessionIdAsync(int userId, string sessionId);
        Task<List<string>> GetUserSessionsAsync(int userId);
        Task<string> GetSessionHistoryAsync(string sessionId); // returns formatted string for AI context
    }

    public class ChatMessageRepository : BaseRepository, IChatMessageRepository
    {
        public ChatMessageRepository(IConfiguration configuration) : base(configuration) { }

        public async Task SaveMessageAsync(int userId, string sessionId, string senderType, string message)
        {
            const string sql = @"
                INSERT INTO ChatMessages (UserId, SessionId, SenderType, Message, IsRead, CreatedDate, IsActive)
                VALUES (@UserId, @SessionId, @SenderType, @Message, 0, @CreatedDate, 1)";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                SessionId = sessionId,
                SenderType = senderType,
                Message = message,
                CreatedDate = DateTime.UtcNow
            });
        }

        public async Task<List<ChatMessage>> GetBySessionIdAsync(int userId, string sessionId)
        {
            const string sql = @"
                SELECT * FROM ChatMessages
                WHERE UserId = @UserId AND SessionId = @SessionId AND IsActive = 1
                ORDER BY CreatedDate ASC";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<ChatMessage>(sql, new { UserId = userId, SessionId = sessionId });
            return result.ToList();
        }

        public async Task<List<string>> GetUserSessionsAsync(int userId)
        {
            const string sql = @"
                SELECT DISTINCT SessionId FROM ChatMessages
                WHERE UserId = @UserId AND IsActive = 1
                ORDER BY MAX(CreatedDate) DESC";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<string>(sql, new { UserId = userId });
            return result.ToList();
        }

        public async Task<string> GetSessionHistoryAsync(string sessionId)
        {
            const string sql = @"
                SELECT TOP 10 SenderType, Message FROM ChatMessages
                WHERE SessionId = @SessionId AND IsActive = 1
                ORDER BY CreatedDate DESC";

            using var connection = CreateConnection();
            var messages = await connection.QueryAsync<ChatMessage>(sql, new { SessionId = sessionId });

            // Format as readable string for AI context
            var sb = new StringBuilder();
            foreach (var msg in messages.Reverse())
                sb.AppendLine($"{msg.SenderType}: {msg.Message}");

            return sb.ToString();
        }
    }
}