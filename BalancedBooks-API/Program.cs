using BalancedBooks_API.Account;
using balancedBooks_API.Authentication;
using BalancedBooks_API.Authentication;
using BalancedBooks_API.Core;
using BalancedBooks_API.Core.Db;
using BalancedBooks_API.Core.Environment;
using BalancedBooks_API.Core.Exceptions;
using BalancedBooks_API.Core.Mediatr;
using BalancedBooks_API.Core.OpenAPI;
using BalancedBooks_API.OpenApi;
using BalancedBooks_API.PublicCompanyCertificate;
using BalancedBooks_Integrations_CompanyRegistry;
using CommunityToolkit.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

/* DEPENDENCIES */

services
    .AddCors()
    // customs
    .AddEnvironmentFlow(configuration)
    .AddAuthenticationConfig(configuration)
    .AddOpenApiDocumentation(configuration)
    .AddMediatrHandlers()
    .AddExceptionMiddlewareSerializer();

services.AddDbConfiguration(configuration);

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

app.UseSwaggerDependencies();
app.UseCors(policyBuilder =>
{
    var config = configuration.GetSection(HttpConfig.ConfigKey).Get<HttpConfig>();
    Guard.IsNotNull(config);

    policyBuilder
        .WithOrigins($"*.{config.MainDomain}")
        .WithExposedHeaders("Set-Cookie")
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionMiddlewareModule();

/* RUN */
var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();

// await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();


app.Run();