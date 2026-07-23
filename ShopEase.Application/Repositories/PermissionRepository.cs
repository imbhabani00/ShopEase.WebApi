using Dapper;
using Microsoft.Extensions.Configuration;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response.Permission;
using System.Data;

namespace Ecommerce.Application.Repositories
{
    #region Interface
    public interface IPermissionRepository
    {
        Task<List<PermissionResponse>> GetByRoleAsync(int roleId);
        Task<bool> SavePermissionsAsync(PermissionRequest request);
    }
    #endregion

    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        #region Constructor
        public PermissionRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        #endregion

        #region GetByRoleAsync
        public async Task<List<PermissionResponse>> GetByRoleAsync(int roleId)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);

                connection.Open();

                var permissions = await connection.QueryAsync<PermissionResponse>(
                    "[dbo].[Permission_GetByRole]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return permissions.ToList();
            }
        }
        #endregion

        #region SavePermissionsAsync
        public async Task<bool> SavePermissionsAsync(PermissionRequest request)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", request.RoleId);
                parameters.Add("@ModuleId", request.ModuleId);
                parameters.Add("@CanView", request.CanView);
                parameters.Add("@CanAdd", request.CanAdd);
                parameters.Add("@CanEdit", request.CanEdit);
                parameters.Add("@CanDelete", request.CanDelete);

                connection.Open();

                var result = await connection.ExecuteAsync(
                    "[dbo].[Permission_SavePermissions]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                connection.Close();

                return result > 0;
            }
        }
        #endregion
    }
}