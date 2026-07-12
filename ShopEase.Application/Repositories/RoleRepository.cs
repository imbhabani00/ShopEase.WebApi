using Dapper;
using Microsoft.Extensions.Configuration;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response.Role;
using ShopEase.Domain.Models;
using System.Data;

namespace Ecommerce.Application.Repositories
{
    #region Interface
    public interface IRoleRepository
    {
        Task<RoleList> GetAllAsync(SortWithPageParameters sortWithPageParameters , int tenantId);
        Task<RoleResponse?> GetByIdAsync(int roleId);
        Task<SaveResponse> InsertAsync(int tenantId, RoleRequest request, int createdBy);
        Task<bool> UpdateAsync(RoleUpdateRequest request, int modifiedBy);
        Task<bool> DeleteAsync(int roleId, int deletedBy);
    }
    #endregion

    public class RoleRepository : BaseRepository, IRoleRepository
    {
        #region Constructor
        public RoleRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region GetAllAsync
        public async Task<RoleList> GetAllAsync(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var data = new RoleList();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@PageNumber", sortWithPageParameters.PageNumber);
                parameters.Add("@PageSize", sortWithPageParameters.PageSize);

                connection.Open();

                var results = await connection.QueryMultipleAsync(
                    "[dbo].[sp_Role_GetAll]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var roles = (await results.ReadAsync<RoleResponse>()).ToList();
                var totalCount = (await results.ReadAsync<int>()).FirstOrDefault();

                connection.Close();
                return data;
            }
        }
        #endregion

        #region GetByIdAsync
        public async Task<RoleResponse?> GetByIdAsync(int roleId)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);

                connection.Open();

                var role = await connection.QueryFirstOrDefaultAsync<RoleResponse>(
                    "[dbo].[sp_Role_GetById]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return role;
            }
        }
        #endregion

        #region InsertAsync
        public async Task<int> InsertAsync(int tenantId, RoleRequest request, int createdBy)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@RoleName", request.RoleName);
                parameters.Add("@RoleCode", request.RoleCode);
                parameters.Add("@Description", request.Description);
                parameters.Add("@CreatedBy", createdBy);
                parameters.Add("@NewRoleId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Open();

                await connection.ExecuteAsync(
                    "[dbo].[sp_Role_Insert]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return parameters.Get<int>("@NewRoleId");
            }
        }
        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(RoleUpdateRequest request, int modifiedBy)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", request.RoleId);
                parameters.Add("@RoleName", request.RoleName);
                parameters.Add("@Description", request.Description);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@ModifiedBy", modifiedBy);

                connection.Open();

                var result = await connection.ExecuteAsync(
                    "[dbo].[sp_Role_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return result > 0;
            }
        }
        #endregion

        #region DeleteAsync
        public async Task<bool> DeleteAsync(int roleId, int deletedBy)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);
                parameters.Add("@DeletedBy", deletedBy);

                connection.Open();

                var result = await connection.ExecuteAsync(
                    "[dbo].[sp_Role_Delete]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return result > 0;
            }
        }
        #endregion
    }
}