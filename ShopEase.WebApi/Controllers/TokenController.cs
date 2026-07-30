using Asp.Versioning;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Models;
using Ecommerce.Domain.Settings;
using Ecommerce.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopEase.Domain.Models.Authuntication;
using System.Net;
using System.Security.Claims;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TokenController : BaseApiController
    {
        #region Properties
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ILogger<TokenController> _logger;
        private readonly JwtSettings _jwtSettings;
        #endregion

        #region Constructor
        public TokenController(
            ITokenService tokenService,
            IUserService userService,
            ILogger<TokenController> logger,
            JwtSettings jwtSettings)
        {
            _tokenService = tokenService;
            _userService = userService;
            _logger = logger;
            _jwtSettings = jwtSettings;
        }
        #endregion

        #region AccessToken
        [HttpPost]
        [Route("access-token")]
        [AllowAnonymous]
        public async Task<IActionResult> AccessToken([FromBody] AuthModel model)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var result = await _userService.AuthenticateAsync(model.Email, model.PasswordHash);
                if (result == null || result.User == null)
                {
                    apiResponse = CreateFailedApiResponse("", HttpStatusCode.NotFound, "Data not found.");
                    return new ObjectResult(apiResponse);
                }
                if (result.ReturnValue != 0)
                {
                    switch (result.ReturnValue)
                    {
                        case 1:
                            apiResponse = CreateFailedApiResponse("", HttpStatusCode.Unauthorized, "Email not found.");
                            break;
                        case 2:
                            apiResponse = CreateFailedApiResponse("", HttpStatusCode.Unauthorized, "Invalid password.");
                            break;
                        case 3:
                            apiResponse = CreateFailedApiResponse("", HttpStatusCode.Unauthorized, "User is inactive.");
                            break;
                        default:
                            apiResponse = CreateFailedApiResponse("", HttpStatusCode.Unauthorized, "Authentication failed.");
                            break;
                    }
                    return new ObjectResult(apiResponse);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
                    new Claim("UserId", result.User.UserId.ToString()),
                    new Claim("RoleId", result.User.RoleId.ToString()),
                    new Claim("RoleName", result.User.RoleName ?? "User"),
                    new Claim(ClaimTypes.Role, result.User.RoleCode ?? "User"),
                    new Claim("RoleCode", result.User.RoleCode ?? "User"),
                    new Claim(ClaimTypes.Email, result.User.Email ?? string.Empty),
                    new Claim("TenantId", result.User.TenantId?.ToString() ?? ""),
                    new Claim("ForcePasswordChange", result.User.ForcePasswordChange.ToString(), ClaimValueTypes.Boolean),
                    new Claim("jti", Guid.NewGuid().ToString())

                };

                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

                await _userService.UpdateRefreshTokenAsync(result.User.UserId, refreshToken, refreshTokenExpiry);

                _logger.LogInformation("AccessToken: User authenticated successfully - UserId: {UserId}", result.User.UserId);

                apiResponse = CreateSuccessResponse(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = result.User.UserId,
                    RoleCode = result.User.RoleCode,
                    RoleName = result.User.RoleName,
                    RoleId = result.User.RoleId,
                    ForcePasswordChange = result.User.ForcePasswordChange
                });
                return new ObjectResult(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse = CreateFailedApiResponse("", HttpStatusCode.InternalServerError, "An error occurred while processing your request");
                _logger.LogError(ex, "AccessToken: Error occurred during authentication");
                return new ObjectResult(apiResponse);
            }
        }
        #endregion

        #region Refresh
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tenantIdClaim = principal.FindFirst("TenantId")?.Value;

                if (!int.TryParse(userIdClaim, out var userId) || !int.TryParse(tenantIdClaim, out var tenantId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Status = false,
                        StatusCode = 401,
                        Message = "Invalid access token",
                        ErrorMessage = "Invalid access token"
                    });
                }

                var user = await _userService.GetByIdAsync(userId , tenantId);
                if (user == null || user.RefreshToken != model.RefreshToken)
                {
                    return Unauthorized(new ApiResponse
                    {
                        Status = false,
                        StatusCode = 401,
                        Message = "Invalid refresh token",
                        ErrorMessage = "Invalid refresh token"
                    });
                }

                var newClaims = principal.Claims.ToList();
                var newAccessToken = _tokenService.GenerateAccessToken(newClaims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

                await _userService.UpdateRefreshTokenAsync(user.UserId.Value, newRefreshToken,refreshTokenExpiry);

                return Ok(new ApiResponse
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Success",
                    Response = new
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    }
                });
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Refresh: Token validation failed");
                return Unauthorized(new ApiResponse
                {
                    Status = false,
                    StatusCode = 401,
                    Message = "Token validation failed",
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh: Error occurred during token refresh");
                return StatusCode(500, new ApiResponse
                {
                    Status = false,
                    StatusCode = 500,
                    Message = "An error occurred while processing your request",
                    ErrorMessage = ex.Message
                });
            }
        }
        #endregion
    }
}