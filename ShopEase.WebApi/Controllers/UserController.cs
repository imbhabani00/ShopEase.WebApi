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

        #region Delete
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var loggedInUserId = User.GetUserId()?.GetHashCode() ?? 0;
                var response = await _userService.DeleteAsync(userId);
                apiResponse = response.ReturnValue == 1
                    ? CreateSuccessResponse(response, HttpStatusCode.OK, "User deleted successfully.")
                    : CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to delete user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete: Error deleting user {UserId}", userId);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to delete user");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region ActiveInactive

        [HttpPut("active-inactive")]
        public async Task<IActionResult> ActiveInactive(int userId, bool isActive)
        {
            var apiResponse = new ApiResponse();

            try
            {
                var response = await _userService.ActiveInactiveAsync(userId, isActive);

                if (response.ReturnValue == 1)
                {
                    string message = isActive
                        ? "User activated successfully"
                        : "User inactivated successfully";

                    apiResponse = CreateSuccessResponse(response, HttpStatusCode.OK, message);
                }
                else
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "Failed to update user status"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveInactive: Error updating user status {UserId}", userId);

                apiResponse = CreateFailedApiResponse(
                    null,
                    HttpStatusCode.InternalServerError,
                    "Failed to update user status"
                );
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

        # region UploadProfilePicture

        [HttpPost("upload-profile-picture")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
        {
            var apiResponse = new ApiResponse();

            try
            {
                var file = request.File;

                if (file == null || file.Length == 0)
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "No file was uploaded."
                    );

                    return new ObjectResult(apiResponse);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "Only JPG, PNG, or WEBP images are allowed."
                    );

                    return new ObjectResult(apiResponse);
                }

                const long maxSizeBytes = 2 * 1024 * 1024;

                if (file.Length > maxSizeBytes)
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "Image must be smaller than 2 MB."
                    );

                    return new ObjectResult(apiResponse);
                }

                var userId = User.GetUserId() ?? 0;
                var tenantId = User.GetTenantId();

                var response = await _userService.UploadProfilePictureAsync(
                    userId,
                    tenantId,
                    file,
                    userId
                );

                apiResponse = response != null
                    ? CreateSuccessResponse(
                        response,
                        HttpStatusCode.OK,
                        "Profile picture updated successfully."
                      )
                    : CreateFailedApiResponse(
                        null,
                        HttpStatusCode.InternalServerError,
                        "Failed to update profile picture."
                      );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadProfilePicture error");

                apiResponse = CreateFailedApiResponse(
                    null,
                    HttpStatusCode.InternalServerError,
                    "An error occurred."
                );
            }

            return new ObjectResult(apiResponse);
        }

        #endregion  

        #region RemoveProfilePicture
        [HttpPost("remove-profile-picture")]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var apiResponse = new ApiResponse();
            try
            {
                var userId = User.GetUserId() ?? 0;
                var tenantId = User.GetTenantId();

                var result = await _userService.RemoveProfilePictureAsync(userId, tenantId, userId);

                apiResponse = result
                    ? CreateSuccessResponse(result, HttpStatusCode.OK, "Profile picture removed successfully.")
                    : CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to remove profile picture.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveProfilePicture error");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "An error occurred.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region LoginGoogle
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest request)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var result = await _userService.AuthenticateGoogleAsync(request.Email);

                if (result == null || result.ReturnValue != 0 || result.User == null)
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.NotFound,
                        "No account found for this Google email. Please register first."
                    );
                }
                else
                {
                    // TODO: issue AccessToken/RefreshToken here the same way your
                    // regular /accesstoken endpoint does, using result.User's claims
                    apiResponse = CreateSuccessResponse(result, HttpStatusCode.OK, "Login successful");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginGoogle: Error logging in via Google for {Email}", request.Email);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to log in with Google.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion
    }
}