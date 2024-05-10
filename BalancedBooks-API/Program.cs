using BalancedBooks_Integrations_CompanyRegistry;
using BalancedBooksAPI.Account;
using BalancedBooksAPI.Authentication;
using BalancedBooksAPI.Authentication.Core;
using BalancedBooksAPI.Core;
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
    .AddCors()
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

await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();


app.Run();