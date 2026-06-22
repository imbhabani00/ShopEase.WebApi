using Asp.Versioning;

namespace Ecommerce.Api.Extensions
{
    public static class VersioningExtension
    {
        public static IServiceCollection AddVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddMvc()     
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";  
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}