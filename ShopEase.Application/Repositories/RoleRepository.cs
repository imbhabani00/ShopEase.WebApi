using Dapper;
using Microsoft.Extensions.Configuration;
using ShopEase.Application.DTOs.Response.Role;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.Role;
using System.Data;

namespace Ecommerce.Application.Repositories
{
    #region Interface
    public interface IRoleRepository
    {
        Task<RoleList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<RoleResponse?> GetById(int roleId);
        Task<SaveResponse> Save(Role role, int tenantId, int userId);
        Task<SaveResponse> Delete(int roleId, int userId);
        Task<PermissionList> GetByRoleAsync(int roleId);
        Task<SaveResponse> SavePermissions(Permission permission);
    }
    #endregion

    public class RoleRepository : BaseRepository, IRoleRepository
    {
        #region Constructor
        public RoleRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region GetList
        public async Task<RoleList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var data = new RoleList();
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
                    "[dbo].[Role_GetList]",
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
            var response = new SaveResponse();

            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@RoleId", role.RoleId);
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@RoleName", role.RoleName);
                parameters.Add("@RoleCode", role.RoleCode);
                parameters.Add("@UserId", userId);

                parameters.Add("@NewRoleId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                await connection.ExecuteAsync(
                    "[dbo].[Role_Save]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                response.ReturnValue = parameters.Get<int>("@ReturnValue");
                response.NewId = parameters.Get<int>("@NewRoleId");
            }

            return response;
        }
        #endregion

        #region Delete
        public async Task<SaveResponse> Delete(int roleId, int userId)
        {
            var response = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);
                parameters.Add("@UserId ", userId);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Open();
                await connection.ExecuteAsync(
                    "[dbo].[Role_Delete]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                response.ReturnValue = parameters.Get<int>("@ReturnValue");
            }
            return response;
        }
        #endregion

        #region GetByRoleAsync
        public async Task<PermissionList> GetByRoleAsync(int roleId)
        {
            var data = new PermissionList();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);

                connection.Open();

                var result = await connection.QueryMultipleAsync(
                    "[dbo].[Permission_GetByRole]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                data.permissions = result.Read<Permission>().ToList();
                data.TotalCount = result.ReadFirstOrDefault<int>();
                return data;
            }
        }
        #endregion

        #region SavePermissionsAsync
        public async Task<SaveResponse> SavePermissions(Permission permission)
        {
            var response = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", permission.RoleId);
                parameters.Add("@ModuleId", permission.ModuleId);
                parameters.Add("@CanView", permission.CanView);
                parameters.Add("@CanAdd", permission.CanAdd);
                parameters.Add("@CanEdit", permission.CanEdit);
                parameters.Add("@CanDelete", permission.CanDelete);
                parameters.Add("@CanInactive", permission.CanInactive);

                connection.Open();

                var result = await connection.ExecuteAsync(
                    "[dbo].[Permission_Save]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return response;
            }
        }
        #endregion
    }
}