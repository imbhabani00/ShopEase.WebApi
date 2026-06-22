namespace Ecommerce.Api.Extensions
{
    public static class CorsExtension
    {
        public static void AddCorsPolicy(
            this IServiceCollection services,
            IConfiguration configuration,
            string policyName)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(allowedOrigins!)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }
    }
}