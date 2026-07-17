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
        Task<RoleList> GetAll(SortWithPageParameters sortWithPageParameters , int tenantId);
        Task<RoleResponse?> GetById(int roleId);
        Task<SaveResponse> Save(Role role, int tenantId, int userId);
        Task<bool> Delete(int roleId, int deletedBy);
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
        public async Task<RoleList> GetAll(SortWithPageParameters sortWithPageParameters, int tenantId)
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
                    "[dbo].[Role_GetAll]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (data.ReturnValue == 0)
                {
                    data.Roles = results.Read<Role>().ToList();
                    data.TotalCount = results.ReadFirstOrDefault<int>();
                }
                return data;
            }
        }
        #endregion

        #region GetById
        public async Task<RoleResponse?> GetById(int roleId)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);

                connection.Open();

                var role = await connection.QueryFirstOrDefaultAsync<RoleResponse>(
                    "[dbo].[Role_GetById]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return role;
            }
        }
        #endregion

        #region Save
        public async Task<SaveResponse> Save(Role role, int tenantId, int userId)
        {
            var data = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleName", role.RoleName);
                parameters.Add("@RoleCode", role.RoleCode);
                parameters.Add("@UserId", userId);
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@NewRoleId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Open();
                await connection.ExecuteAsync(
                    "[dbo].[Role_Save]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                connection.Close();
                return data;
            }
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(int roleId, int deletedBy)
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