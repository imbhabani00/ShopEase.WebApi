using Asp.Versioning;
using Ecommerce.Api.Helper;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request;
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
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var tenantId = User.GetTenantId() ?? 0;
                var roles = await _roleService.GetAllAsync(tenantId, pageNumber, pageSize);

                var apiResponse = CreateSuccessResponse(roles, HttpStatusCode.OK, "Roles retrieved successfully");
                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll: Error retrieving roles");
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve roles");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);

                if (role == null)
                {
                    var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.NotFound, "Role not found");
                    return new ObjectResult(apiResponse);
                }

                var response = CreateSuccessResponse(role, HttpStatusCode.OK, "Role retrieved successfully");
                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById: Error retrieving role {RoleId}", id);
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve role");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion

        #region Create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoleRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RoleName))
                {
                    var validationError = CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Role name is required");
                    return new ObjectResult(validationError);
                }

                var tenantId = User.GetTenantId() ?? 0;
                var userId = User.GetUserId()?.GetHashCode() ?? 0;

                var response = await _roleService.CreateAsync(tenantId, request, userId);

                if (response.ReturnValue == 0)
                {
                    var apiResponse = CreateSuccessResponse(response.NewId, HttpStatusCode.Created, "Role created successfully");
                    return new ObjectResult(apiResponse);
                }
                else
                {
                    var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to create role");
                    return new ObjectResult(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create: Error creating role");
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to create role");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion

        #region Update
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] RoleUpdateRequest request)
        {
            try
            {
                if (request.RoleId <= 0 || string.IsNullOrEmpty(request.RoleName))
                {
                    var validationError = CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Invalid role data");
                    return new ObjectResult(validationError);
                }

                var userId = User.GetUserId()?.GetHashCode() ?? 0;
                var apiResponse = await _roleService.UpdateAsync(request, userId);

                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update: Error updating role");
                var apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to update role");
                return new ObjectResult(apiResponse);
            }
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