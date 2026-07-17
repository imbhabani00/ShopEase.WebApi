using Asp.Versioning;
using AutoMapper;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Service;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Domain.Models;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var loggedInUserId = CurrentUserId ?? 0;
                var tenantId = User.GetTenantId() == 0 ? 1 : User.GetTenantId();
                var response = await _userService.SaveAsync(register, tenantId , loggedInUserId);

                switch (response.ReturnValue)
                {
                    case 0:
                        apiResponse = CreateSuccessResponse(response.NewId, HttpStatusCode.OK,
                            response.NewId > 0 ? "Registration successful." : "Registration failed.");
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
    }
}