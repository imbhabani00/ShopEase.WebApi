using Dapper;
using Ecommerce.Domain.Models;
using Microsoft.Extensions.Configuration;
using ShopEase.Domain.Models;
using System.Data;

namespace Ecommerce.Application.Repositories
{
    #region Interface
    public interface IUserRepository
    {
        Task<UserGet?> Authenticate(string email, string password);
        Task<UserGet?> GetById(int userId);
        Task UpdateRefreshToken(int userId, string refreshToken, DateTime refreshTokenExpiry);
        Task<SaveResponse> Save(Register register, int tenantId, int loggedInUserId);
        Task<UsersList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId);
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

        #region Save
        public async Task<SaveResponse> Save(Register register, int tenantId, int loggedInUserId)
        {
            var response = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", register.UserId);
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@LoggedInUserId", loggedInUserId);
                parameters.Add("@FirstName", register.FirstName);
                parameters.Add("@MiddleName", register.MiddleName);
                parameters.Add("@LastName", register.LastName);
                parameters.Add("@Email", register.Email);
                parameters.Add("@PhoneNumber", register.PhoneNumber);
                parameters.Add("@PasswordHash", register.PasswordHash);
                parameters.Add("@RoleId", register.RoleId);
                parameters.Add("@IsActive", register.IsActive);
                parameters.Add("@NewUserId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                connection.Open();
                await connection.ExecuteAsync("[dbo].[Users_Save]", parameters, commandType: CommandType.StoredProcedure);
                response.ReturnValue = parameters.Get<int>("@ReturnValue");
                if (response.ReturnValue == 0)
                {
                    response.NewId = parameters.Get<int>("@NewUserId");
                }
            }
            return response;
        }
        #endregion

        #region GetList
        public async Task<UsersList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var data = new UsersList();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@PageNumber", sortWithPageParameters.PageNumber);
                parameters.Add("@PageSize", sortWithPageParameters.PageSize);
                parameters.Add("@SearchString", sortWithPageParameters.SearchString);
                parameters.Add("@SortParameter", sortWithPageParameters.SortParameter);
                parameters.Add("@SortDirection", sortWithPageParameters.SortDirection);

                connection.Open();

                var results = await connection.QueryMultipleAsync(
                    "[dbo].[Users_GetList]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (data.ReturnValue == 0)
                {
                    data.UsersData = results.Read<Users>().ToList();
                    data.TotalCount = results.ReadFirstOrDefault<int>();
                }
                return data;
            }
        }
        #endregion
    }
}