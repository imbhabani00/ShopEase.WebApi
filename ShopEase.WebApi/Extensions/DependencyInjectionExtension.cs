using Ecommerce.Domain.Settings;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddSingleton(jwtSettings);
        var appAssembly = typeof(Ecommerce.Application.AssemblyReference).Assembly;

        foreach (var type in appAssembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.Name.EndsWith("Service")))
        {
            // Register all interfaces implemented by the service
            var interfaceTypes = type.GetInterfaces()
                .Where(i => i.Name != "IDisposable");

            foreach (var interfaceType in interfaceTypes)
            {
                bool needsHttpClient = type.GetConstructors()
                    .Any(c => c.GetParameters()
                        .Any(p => p.ParameterType == typeof(HttpClient)));

                if (needsHttpClient)
                {
                    services.AddHttpClient(type.Name);
                    services.AddScoped(interfaceType, type);
                }
                else
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }

        // Repositories loop stays the same
        foreach (var type in appAssembly.GetTypes()
            .Where(x => x.IsClass
                     && !x.IsAbstract
                     && x.Name.EndsWith("Repository")
                     && x.Name != "BaseRepository"))
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name != "IDisposable"
                                 && i.Name != "IBaseRepository");

            if (interfaceType != null)
                services.AddScoped(interfaceType, type);
        }

        return services;
    }
}
