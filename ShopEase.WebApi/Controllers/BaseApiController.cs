using Ecommerce.Api.Helper;
using Ecommerce.Application.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public int UserId { get; set; }  
        public int TenantId { get; set; }
        public int TenantCode { get; set; }
        public int AppCode { get; set; }
        public string? RoleCode { get; set; }
        public int RoleId { get; set; }
        protected int? CurrentUserId => User.GetUserId().HasValue? User.GetUserId().GetHashCode(): null;
        protected string? CurrentUserEmail => User.GetUserEmail();
        protected string? CurrentUserRole => User.GetUserRole();

        protected ApiResponse BuildValidationErrorApiResponse(FluentValidation.Results.ValidationResult validateResult)
        {
            var validationErrors = new List<ValidationError>();
            var errors = validateResult.Errors;

            foreach (var error in errors)
            {
                validationErrors.Add(new ValidationError
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage,
                    PropertyName = error.PropertyName
                });
            }

            return new ApiResponse
            {
                Response = validationErrors,
                ErrorMessage = "Validation Errors",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Status = false,
                Message = "Failed"
            };
        }

        protected ApiResponse CreateSuccessResponse(object response, HttpStatusCode statusCode = HttpStatusCode.OK, string message = "Success")
        {
            return new ApiResponse
            {
                Response = response,
                ErrorMessage = "",
                StatusCode = (int)statusCode,
                Status = true,
                Message = message
            };
        }

        protected ApiResponse CreateFailedApiResponse(object response, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string message = "Failed")
        {
            return new ApiResponse
            {
                Response = response,
                ErrorMessage = "",
                StatusCode = (int)statusCode,
                Status = false,
                Message = message
            };
        }
    }
}