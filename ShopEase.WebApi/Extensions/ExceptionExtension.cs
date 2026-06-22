using Ecommerce.Api.Middleware;

namespace Ecommerce.Api.Extensions
{
    public static class ExceptionExtension
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
