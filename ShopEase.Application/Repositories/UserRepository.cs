using Dapper;
using Ecommerce.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Ecommerce.Application.Repositories
{
    #region Interface
    public interface IUserRepository
    {
        Task<UserGet?> Authenticate(string email, string password);
        Task<UserGet?> GetById(int userId);
        Task UpdateRefreshToken(int userId, string refreshToken , DateTime refreshTokenExpiry);
    }
    #endregion

    public class UserRepository : BaseRepository, IUserRepository
    {
        #region Constructor
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region Authenticate
        public async Task<UserGet?> Authenticate(string email, string password)
        {
            var data = new UserGet();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);
                parameters.Add("@PasswordHash", password);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                connection.Open();
                var result = await connection.QueryMultipleAsync("[dbo].[User_Authenticate]", parameters, commandType: CommandType.StoredProcedure);
                data.User = await result.ReadFirstOrDefaultAsync<User>();
                data.ReturnValue = parameters.Get<int>("@ReturnValue");
                connection.Close();
            }
            return data;
        }
        #endregion

        #region UpdateRefreshToken
        public async Task UpdateRefreshToken(int userId, string refreshToken, DateTime refreshTokenExpiry)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@RefreshToken", refreshToken);
                parameters.Add("@RefreshTokenExpiry", refreshTokenExpiry);
                connection.Open();
                await connection.ExecuteAsync("[dbo].[Users_UpdateRefreshToken]", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion

        #region GetById
        public async Task<UserGet?> GetById(int userId)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                connection.Open();
                var result = await connection.QueryMultipleAsync("", parameters, commandType: CommandType.StoredProcedure);
                var user = await result.ReadFirstOrDefaultAsync<UserGet>();
                var returnValue = parameters.Get<int>("@ReturnValue");
                return user;
            }
        }
        #endregion
    }
}