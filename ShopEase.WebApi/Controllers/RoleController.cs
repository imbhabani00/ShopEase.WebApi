using Asp.Versioning;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Request.Role;
using ShopEase.Domain.Models;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RoleController : BaseApiController
    {
        #region Properties
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        #endregion

        #region Constructor
        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }
        #endregion

        #region GetList
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] SortWithPageParameters sortWithPageParameters)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var tenantId = User.GetTenantId();
                var roles = await _roleService.GetListAsync(sortWithPageParameters, tenantId);
                apiResponse = CreateSuccessResponse(roles, HttpStatusCode.OK, "Roles retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll: Error retrieving roles");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve roles");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region GetById
        [HttpGet("role-by-id/{roleId}")]
        public async Task<IActionResult> GetById(int roleId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var response = await _roleService.GetByIdAsync(roleId);
                apiResponse = response == null
                    ? CreateFailedApiResponse(null, HttpStatusCode.NotFound, "Role not found")
                    : CreateSuccessResponse(response, HttpStatusCode.OK, "Role retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById: Error retrieving role {RoleId}", roleId);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve role");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] RoleRequest request)
        {
            var apiResponse = new ApiResponse();

            try
            {
                var tenantId = User.GetTenantId();
                var userId = User.GetUserId() ?? 0;

                var response = await _roleService.SaveAsync(request, tenantId, userId);

                switch (response.ReturnValue)
                {
                    case 0:
                        apiResponse = CreateSuccessResponse(response.NewId,
                            HttpStatusCode.OK,
                            response.NewId > 0 ? "Role saved successfully." : "Role updated successfully.");
                        break;
                    case 1:
                        apiResponse = CreateFailedApiResponse(
                            null,
                            HttpStatusCode.BadRequest,
                            "Role name already exists."
                        );
                        break;

                    case 2:
                        apiResponse = CreateFailedApiResponse(
                            null,
                            HttpStatusCode.BadRequest,
                            "Role code already exists."
                        );
                        break;
                    default:
                        apiResponse = CreateFailedApiResponse(
                            null,
                            HttpStatusCode.InternalServerError,
                            "Internal server error.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save: Error saving role");

                apiResponse = CreateFailedApiResponse(
                    null,
                    HttpStatusCode.InternalServerError,
                    "Failed to save role."
                );
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region Delete
        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> Delete(int roleId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var userId = User.GetUserId()?.GetHashCode() ?? 0;
                var response = await _roleService.DeleteAsync(roleId, userId);
                apiResponse = response.ReturnValue == 1
                    ? CreateSuccessResponse(response, HttpStatusCode.OK, "Role deleted successfully")
                    : CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to delete role");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete: Error deleting role {RoleId}", roleId);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to delete role");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region ActiveInactive

        [HttpPut("active-inactive")]
        public async Task<IActionResult> ActiveInactive(int roleId, bool isActive)
        {
            var apiResponse = new ApiResponse();

            try
            {
                var response = await _roleService.ActiveInactiveAsync(roleId, isActive);

                if (response.ReturnValue == 1)
                {
                    string message = isActive
                        ? "Role activated successfully"
                        : "Role inactivated successfully";

                    apiResponse = CreateSuccessResponse(response, HttpStatusCode.OK, message);
                }
                else
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "Failed to update role status"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveInactive: Error updating role status {Roleid}", roleId);

                apiResponse = CreateFailedApiResponse(
                    null,
                    HttpStatusCode.InternalServerError,
                    "Failed to update role status"
                );
            }

            return new ObjectResult(apiResponse);
        }

        #endregion

        #region GetByRole
        [HttpGet("by-role/{roleId}")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var permissions = await _roleService.GetByRoleAsync(roleId);
                apiResponse = CreateSuccessResponse(permissions, HttpStatusCode.OK, "Permissions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByRole: Error retrieving permissions for role {RoleId}", roleId);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve permissions");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region PermissionSave

        [HttpPost("permission-save")]
        public async Task<IActionResult> PermissionSave([FromBody] List<PermissionRequest> request)
        {
            var apiResponse = new ApiResponse();
            try
            {
                if (request == null || request.Count == 0)
                {
                    var validationError = CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Invalid permissions");
                    return new ObjectResult(validationError);
                }

                apiResponse = await _roleService.SavePermissionsAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionSave: Error saving permissions");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to save permissions");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion
    }
}