using Dapper;
using Ecommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<int> CreateAsync(PaymentTransaction transaction);
        Task<PaymentTransaction> GetByOrderIdAsync(int orderId);
        Task<PaymentTransaction> GetByGatewayOrderIdAsync(string gatewayOrderId);
        Task UpdateStatusAsync(string gatewayOrderId, string status, string gatewayPaymentId, string signature);
        Task UpdateRefundAsync(string gatewayPaymentId, string refundId, decimal refundAmount);
    }
    public class PaymentTransactionRepository : BaseRepository, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int> CreateAsync(PaymentTransaction transaction)
        {
            const string sql = @"
                INSERT INTO PaymentTransactions 
                    (OrderId, UserId, GatewayOrderId, Amount, Currency, Status, CreatedDate, IsActive)
                VALUES 
                    (@OrderId, @UserId, @GatewayOrderId, @Amount, @Currency, @Status, @CreatedDate, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = CreateConnection();
            return await connection.QuerySingleAsync<int>(sql, transaction);
        }

        public async Task<PaymentTransaction> GetByOrderIdAsync(int orderId)
        {
            const string sql = @"
                SELECT * FROM PaymentTransactions
                WHERE OrderId = @OrderId AND IsActive = 1
                ORDER BY CreatedDate DESC";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PaymentTransaction>(sql, new { OrderId = orderId });
        }

        public async Task<PaymentTransaction> GetByGatewayOrderIdAsync(string gatewayOrderId)
        {
            const string sql = @"
                SELECT * FROM PaymentTransactions
                WHERE GatewayOrderId = @GatewayOrderId AND IsActive = 1";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PaymentTransaction>(sql, new { GatewayOrderId = gatewayOrderId });
        }

        public async Task UpdateStatusAsync(string gatewayOrderId, string status, string gatewayPaymentId, string signature)
        {
            const string sql = @"
                UPDATE PaymentTransactions
                SET Status             = @Status,
                    GatewayPaymentId   = @GatewayPaymentId,
                    GatewaySignature   = @GatewaySignature,
                    PaidAt             = CASE WHEN @Status = 'Captured' THEN GETUTCDATE() ELSE NULL END,
                    ModifiedDate       = GETUTCDATE()
                WHERE GatewayOrderId = @GatewayOrderId";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                GatewayOrderId = gatewayOrderId,
                Status = status,
                GatewayPaymentId = gatewayPaymentId,
                GatewaySignature = signature
            });
        }

        public async Task UpdateRefundAsync(string gatewayPaymentId, string refundId, decimal refundAmount)
        {
            const string sql = @"
                UPDATE PaymentTransactions
                SET Status        = 'Refunded',
                    RefundId      = @RefundId,
                    RefundAmount  = @RefundAmount,
                    ModifiedDate  = GETUTCDATE()
                WHERE GatewayPaymentId = @GatewayPaymentId";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                GatewayPaymentId = gatewayPaymentId,
                RefundId = refundId,
                RefundAmount = refundAmount
            });
        }
    }
}