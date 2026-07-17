using Asp.Versioning;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request;
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

        #region GetAll
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] SortWithPageParameters sortWithPageParameters)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var tenantId = User.GetTenantId();
                var roles = await _roleService.GetAllAsync(sortWithPageParameters, tenantId);
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
                var userId = User.GetUserId()?.GetHashCode() ?? 0;

                var response = await _roleService.SaveAsync(request, tenantId, userId);

                switch (response.ReturnValue)
                {
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
                        apiResponse = response.NewId > 0
                            ? CreateSuccessResponse(
                                response.NewId,
                                HttpStatusCode.OK,
                                request.RoleId > 0
                                    ? "Role updated successfully."
                                    : "Role created successfully."
                            )
                            : CreateFailedApiResponse(
                                null,
                                HttpStatusCode.BadRequest,
                                "Failed to save role."
                            );
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.GetUserId()?.GetHashCode() ?? 0;
                var apiResponse = await _roleService.DeleteAsync(id, userId);

                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete: Error deleting role {RoleId}", id);
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to delete role");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion
    }
}