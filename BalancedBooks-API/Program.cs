using BalancedBooks_Integrations_CompanyRegistry;
using BalancedBooksAPI.Account;
using BalancedBooksAPI.Authentication;
using BalancedBooksAPI.Authentication.Core;
using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Environment;
using BalancedBooksAPI.Core.Exceptions;
using BalancedBooksAPI.Core.Mediatr;
using BalancedBooksAPI.Core.OpenAPI;
using BalancedBooksAPI.OpenApi;
using BalancedBooksAPI.PublicCompanyCertificate;
using CommunityToolkit.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

/* DEPENDENCIES */

services
    .AddCors(options =>
    {
        options.AddPolicy("CorsFrontend", policyBuilder =>
        {
            var authConfig = configuration.GetSection(AuthConfig.ConfigKey).Get<AuthConfig>();
            Guard.IsNotNull(authConfig);

            policyBuilder
                .WithOrigins($"*.{authConfig.Domain}")
                .WithExposedHeaders("Set-Cookie")
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader();
        });
    })
    // customs
    .AddEnvironmentFlow(configuration)
    .AddAuthenticationConfig(configuration)
    .AddOpenApiDocumentation(configuration)
    .AddMediatrHandlers()
    .AddExceptionMiddlewareSerializer();

services.AddDbConfiguration(configuration);
services.AddSingleton<AuthenticationService>();

/*
 * Company Registry Module
 */

var companyRegistryConfig = configuration
    .GetSection(CompanyRegistryConfig.ConfigKey)
    .Get<CompanyRegistryConfig>();

Guard.IsNotNull(companyRegistryConfig);

services.AddHttpContextAccessor();
services.AddCompanyRegistryIntegrationModule(companyRegistryConfig);

// init
var app = builder.Build();

/* MODULES */

app.MapAuthenticationModuleRoutes();
app.MapAccountModuleRoutes();
app.MapPublicCompanyCertificateModuleRoutes();
app.MapOpenApiModuleRoutes();

/* MIDDLEWARES */

app.Use(async (context, next) =>
{
    
    await next(context);
});

app.UseSwaggerDependencies();
app.UseCors("CorsFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionMiddlewareModule();
app.UseHttpsRedirection();

/* RUN */
var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();

await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();


app.Run();