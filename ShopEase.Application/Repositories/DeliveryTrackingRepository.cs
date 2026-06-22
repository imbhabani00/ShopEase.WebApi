using Dapper;
using Ecommerce.Application.Repositories;
using Ecommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Repositories
{
     public interface IDeliveryTrackingRepository
    {
        Task SaveLocationAsync(DeliveryTracking tracking);
        Task<DeliveryTracking> GetLatestByOrderIdAsync(int orderId);
        Task<List<DeliveryTracking>> GetHistoryByOrderIdAsync(int orderId);
    }

    public class DeliveryTrackingRepository : BaseRepository, IDeliveryTrackingRepository
    {
        public DeliveryTrackingRepository(IConfiguration configuration) : base(configuration) { }

        public async Task SaveLocationAsync(DeliveryTracking tracking)
        {
            const string sql = @"
                INSERT INTO DeliveryTrackings 
                    (OrderId, DeliveryAgentId, Latitude, Longitude, FormattedAddress, Status, TrackedAt, CreatedDate, IsActive)
                VALUES 
                    (@OrderId, @DeliveryAgentId, @Latitude, @Longitude, @FormattedAddress, @Status, @TrackedAt, @CreatedDate, 1)";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, tracking);
        }

        public async Task<DeliveryTracking> GetLatestByOrderIdAsync(int orderId)
        {
            const string sql = @"
                SELECT TOP 1 * FROM DeliveryTrackings
                WHERE OrderId = @OrderId AND IsActive = 1
                ORDER BY TrackedAt DESC";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DeliveryTracking>(sql, new { OrderId = orderId });
        }

        public async Task<List<DeliveryTracking>> GetHistoryByOrderIdAsync(int orderId)
        {
            const string sql = @"
                SELECT * FROM DeliveryTrackings
                WHERE OrderId = @OrderId AND IsActive = 1
                ORDER BY TrackedAt ASC";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<DeliveryTracking>(sql, new { OrderId = orderId });
            return result.ToList();
        }
    }
}