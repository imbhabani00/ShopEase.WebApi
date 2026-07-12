using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Repositories;
using Microsoft.Extensions.Logging;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response.Permission;

namespace Ecommerce.Application.Services
{
   
    #region IPermissionService
    public interface IPermissionService
    {
        Task<List<PermissionResponse>> GetByRoleAsync(int roleId);
        Task<ApiResponse> SaveAsync(PermissionRequest request);
    }
    #endregion

    public class PermissionService : IPermissionService
    {
        #region Properties
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<PermissionService> _logger;
        #endregion

        #region Constructor
        public PermissionService(IPermissionRepository permissionRepository, ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _logger = logger;
        }
        #endregion

        #region GetByRoleAsync
        public async Task<List<PermissionResponse>> GetByRoleAsync(int roleId)
        {
            try
            {
                return await _permissionRepository.GetByRoleAsync(roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByRoleAsync: Error retrieving permissions for role {RoleId}", roleId);
                return new List<PermissionResponse>();
            }
        }
        #endregion

        #region SaveAsync
        public async Task<ApiResponse> SaveAsync(PermissionRequest request)
        {
            try
            {
                var saved = await _permissionRepository.SavePermissionsAsync(request);

                if (saved)
                {
                    _logger.LogInformation("SaveAsync: Permission saved successfully for Role {RoleId} Module {ModuleId}",
                        request.RoleId, request.ModuleId);
                    return new ApiResponse
                    {
                        Status = true,
                        Message = "Permission saved successfully"
                    };
                }
                else
                {
                    return new ApiResponse
                    {
                        Status = false,
                        Message = "Failed to save permission"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveAsync: Error saving permission");
                return new ApiResponse
                {
                    Status = false,
                    Message = "An error occurred while saving permission"
                };
            }
        }
        #endregion
    }
}