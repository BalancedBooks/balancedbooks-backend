using BalancedBooks_API.Authentication;
using BalancedBooks_API.Core;
using BalancedBooks_API.Core.Environment;
using BalancedBooks_API.Core.Exceptions;
using BalancedBooks_API.Core.Mediatr;
using BalancedBooks_API.Core.OpenAPI;
using BalancedBooks_API.ExternalIntegrations.CompanyRegistry;
using BalancedBooks_API.OpenApi;
using BalancedBooks_API.PublicCompanyCertificate;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

/* DEPENDENCIES */

services
    .AddCors()
    // customs
    .AddEnvironmentFlow(configuration)
    .AddJwtAuthentication(configuration)
    .AddOpenApiDocumentation(configuration)
    .AddMediatrHandlers()
    .AddExceptionMiddlewareSerializer()
    .AddCompanyRegistryIntegration(configuration);

// init
var app = builder.Build();

/* MODULES */

app.MapAuthenticationModuleRoutes();
app.MapPublicCompanyCertificateModuleRoutes();
app.MapOpenApiModuleRoutes();

/* MIDDLEWARES */

app.UseSwaggerDependencies();
app.UseCors(policyBuilder =>
{
    policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionMiddlewareModule();

/* RUN */

app.Run();