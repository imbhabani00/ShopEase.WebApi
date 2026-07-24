using Asp.Versioning;
using Ecommerce.Api.Controllers;
using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using ShopEase.Application.Services;
using System.Net;

namespace ShopEase.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LookupController : BaseApiController
    {
        #region Properties
        private readonly ILookupService _lookupService;
        private readonly ILogger<LookupController> _logger;

        #endregion

        #region Constructor
        public LookupController(ILookupService lookupService , ILogger<LookupController> logger)
        {
            _lookupService = lookupService;
            _logger = logger;
        }
        #endregion

        #region GetRoles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var apiResponse = new ApiResponse();
            try
            {
                var tenantId = User.GetTenantId();
                var result = await _lookupService.GetRolesAsync(tenantId);
                apiResponse = result != null ? CreateSuccessResponse(result , HttpStatusCode.OK , "Roles loaded successfully.") :
                    CreateFailedApiResponse(null , HttpStatusCode.NotFound , "Roles not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex ,"Error occure while executing roles list.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion
    }
}