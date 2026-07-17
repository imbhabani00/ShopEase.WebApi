using Asp.Versioning;
using Ecommerce.Api.Controllers;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request;
using System.Net;

namespace ShopEase.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PermissionController : BaseApiController
    {
        #region Properties
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;
        #endregion

        #region Constructor
        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }
        #endregion

        #region GetByRole
        [HttpGet("by-role/{roleId}")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            try
            {
                var permissions = await _permissionService.GetByRoleAsync(roleId);

                var apiResponse = CreateSuccessResponse(permissions, HttpStatusCode.OK, "Permissions retrieved successfully");
                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByRole: Error retrieving permissions for role {RoleId}", roleId);
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve permissions");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] PermissionRequest request)
        {
            try
            {
                if (request.RoleId <= 0 || request.ModuleId <= 0)
                {
                    var validationError = CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Invalid role or module");
                    return new ObjectResult(validationError);
                }

                var apiResponse = await _permissionService.SaveAsync(request);

                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save: Error saving permission");
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to save permission");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion
    }
}