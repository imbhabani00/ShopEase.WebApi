using Asp.Versioning;
using AutoMapper;
using Ecommerce.Api.Controllers;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.DTOs.Request.user;
using ShopEase.Application.Services;
using ShopEase.Domain.Models;
using System.Net;

namespace ShopEase.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomerController : BaseApiController
    {
        #region Properties
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICustomerService _customerService;
        #endregion

        #region Constructor
        public CustomerController(IUserService userService,
            IMapper mapper,
            ILogger<CustomerController> logger,
            ICustomerService customerService,
            IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _customerService = customerService;
        }
        #endregion

        #region RegisterCustomer
        [HttpPost("register-customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] UserRequest userRequest)
        {
            var apiResponse = new ApiResponse();
            try
            {
                userRequest.RoleId = _configuration.GetValue<int>("AppSettings:CustomerRoleId");
                var tenantId = _configuration.GetValue<int>("AppSettings:DefaultTenantId");

                var response = await _userService.SaveAsync(userRequest, tenantId, loggedInUserId: 0);

                switch (response.ReturnValue)
                {
                    case 0:
                        apiResponse = CreateSuccessResponse(response.NewId, HttpStatusCode.OK, "Registration successful.");
                        break;
                    case 1:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.Conflict, "User already exists.");
                        break;
                    case 2:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.Conflict, "Email already exists.");
                        break;
                    default:
                        apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Something went wrong.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterCustomer error");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "An error occurred.");
            }
            return new ObjectResult(apiResponse);
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
                var roles = await _customerService.GetListAsync(sortWithPageParameters, tenantId);
                apiResponse = CreateSuccessResponse(roles, HttpStatusCode.OK, "Customers retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll: Error retrieving customers.");
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to retrieve customers.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region Delete
        [HttpDelete("delete/{customerId}")]
        public async Task<IActionResult> Delete(int customerId)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var loggedInUserId = User.GetUserId()?.GetHashCode() ?? 0;
                var response = await _customerService.DeleteAsync(customerId);
                apiResponse = response.ReturnValue == 1
                    ? CreateSuccessResponse(response, HttpStatusCode.OK, "Customer deleted successfully")
                    : CreateFailedApiResponse(null, HttpStatusCode.BadRequest, "Failed to delete customer");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete: Error deleting customer {CustomerId}", customerId);
                apiResponse = CreateFailedApiResponse(null, HttpStatusCode.InternalServerError, "Failed to delete customer");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region ActiveInactive

        [HttpPut("active-inactive")]
        public async Task<IActionResult> ActiveInactive(int customerId, bool isActive)
        {
            var apiResponse = new ApiResponse();

            try
            {
                var response = await _customerService.ActiveInactiveAsync(customerId, isActive);

                if (response.ReturnValue == 1)
                {
                    string message = isActive
                        ? "Customer activated successfully"
                        : "Customer inactivated successfully";

                    apiResponse = CreateSuccessResponse(response, HttpStatusCode.OK, message);
                }
                else
                {
                    apiResponse = CreateFailedApiResponse(
                        null,
                        HttpStatusCode.BadRequest,
                        "Failed to update customer status"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveInactive: Error updating customer status {CustomerId}", customerId);

                apiResponse = CreateFailedApiResponse(
                    null,
                    HttpStatusCode.InternalServerError,
                    "Failed to update customer status"
                );
            }

            return new ObjectResult(apiResponse);
        }

        #endregion
    }
}