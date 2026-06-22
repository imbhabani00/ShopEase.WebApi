using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Ecommerce.Infrastructure.Repositories
{
    public interface IBaseRepository {
        IDbConnection CreateConnection();
    }

    public class BaseRepository : IBaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("EcommerceConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}