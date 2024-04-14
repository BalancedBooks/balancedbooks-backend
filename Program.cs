using balancedbooks_backend.Authentication;
using balancedbooks_backend.Core;
using balancedbooks_backend.Core.Exceptions;
using balancedbooks_backend.Core.Mediatr;
using balancedbooks_backend.Core.OpenAPI;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddEnvironmentFlowDeps();

var httpConfig = services.AddHttpConfig(configuration);
services.AddAuthenticationDeps(configuration);

services.AddSwaggerDeps(httpConfig);

services.AddMediatrDeps();

services.AddCors();

services.AddExceptionMiddlewareModule();

/* INIT */

var app = builder.Build();

app.MapGroup("/auth").MapAuthenticationRoutes();

app.UseSwaggerDeps();

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

app.Run();