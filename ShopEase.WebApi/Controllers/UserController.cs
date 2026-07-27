using Asp.Versioning;
using AutoMapper;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Service;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request.user;
using ShopEase.Domain.Models;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : BaseApiController
    {
        #region Properties
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        #endregion

        #region Constructor
        public UserController(IUserService userService,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
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
                var users = await _userService.GetListAsync(sortWithPageParameters, tenantId);
                apiResponse = CreateSuccessResponse(users, HttpStatusCode.OK, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll: Error retrieving users");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve users");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] UserRequest userRequest)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var loggedInUserId = User.GetUserId() ?? 0;
                var tenantId = User.GetTenantId();
                var response = await _userService.SaveAsync(userRequest, tenantId, loggedInUserId);

                switch (response.ReturnValue)
                {
                    case 0:
                        apiResponse = CreateSuccessResponse(response.NewId, HttpStatusCode.OK,
                            response.NewId > 0 ? "User saved successfully." : "User updated successfully.");
                        break;
                    case 1:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.Conflict,
                            "User is already exist.");
                        break;
                    case 2:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.Conflict,
                            "Email already exists.");
                        break;
                    default:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError,
                            "Something went wrong.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError,
                    "An error occurred.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region GetById
        [HttpGet("id/{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var tenantId = User.GetTenantId();
                var response = await _userService.GetByIdAsync(userId, tenantId);
                apiResponse = response != null
                    ? CreateSuccessResponse(response, HttpStatusCode.OK, "User retrieved successfully")
                    : CreateFailedApiResponse(null, HttpStatusCode.NotFound, "User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById: Error retrieving user {UserId}", userId);
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region ChangePassword
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var result = await _userService.ChangePasswordAsync(request.UserId, request.PasswordHash);
                apiResponse = result != null
                    ? CreateSuccessResponse(result, HttpStatusCode.OK, "Password changed successfully.")
                    : CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to change password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePassword error");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "An error occurred.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion
    }
}