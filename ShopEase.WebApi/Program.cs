using Ecommerce.Api.Extensions;
using Ecommerce.Api.Hubs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using AutoMapper;
using Ecommerce.Application;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Ecommerce API...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((context, services, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext()
              .Enrich.WithMachineName()
              .Enrich.WithEnvironmentName()
              .WriteTo.Console()
              .WriteTo.MSSqlServer(
                  connectionString: context.Configuration
                          .GetConnectionString("ShopEaseConnection"),
                  sinkOptions: new MSSqlServerSinkOptions
                  {
                      TableName = "tbl_Logs",
                      SchemaName = "dbo",
                      AutoCreateSqlTable = true
                  });
    });

    // Controllers + Swagger
    builder.Services.AddControllers();
    builder.Services.AddSwaggerExtension();

    // CORS
    builder.Services.AddCorsPolicy(builder.Configuration,"CorsPolicy");

    // JWT Authentication
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
        options.AddPolicy("BuyerOnly", policy => policy.RequireRole("Buyer"));
        options.AddPolicy("AdminOrSeller", policy => policy.RequireRole("Admin", "Seller"));
    });

    // FluentValidation
    builder.Services.AddValidatorsFromAssembly(
        typeof(Ecommerce.Application.AssemblyReference).Assembly);
    builder.Services.AddFluentValidationAutoValidation();

    //AutoMapper
    builder.Services.AddAutoMapper(typeof(AssemblyReference).Assembly);

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("ShopEaseConnection")!);

    // API Versioning
    builder.Services.AddVersioningExtension();

    // SignalR
    builder.Services.AddSignalR();

    // Dependency Injection
    builder.Services.AddDependencyInjection(builder.Configuration);

    // Routing
    builder.Services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });

    var app = builder.Build();

    // ── Middleware Pipeline ──────────────────
    app.UseExceptionHandling();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    //app.UseResponseCaching();
    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    // ── Endpoints ───────────────────────────
    app.MapControllers();
    app.MapHub<ChatHub>("/hubs/chat");
    app.MapHub<OrderTrackingHub>("/hubs/tracking");
    app.MapHealthChecks("/health");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API failed to start");
}
finally
{
    Log.CloseAndFlush();
}