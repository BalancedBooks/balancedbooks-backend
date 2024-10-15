using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Environment;
using BalancedBooksAPI.Core.Exceptions;
using BalancedBooksAPI.Core.Mediatr;
using BalancedBooksAPI.Core.OpenApi.DocumentTransformers;
using BalancedBooksAPI.Features.Authentication.Config;
using BalancedBooksAPI.Features.Authentication.Services;
using Carter;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Routing.Constraints;

var builder = WebApplication.CreateEmptyBuilder(new()
{
    EnvironmentName = "development"
});

var services = builder.Services;
var configuration = builder.Configuration;

// swagger uses regex
services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.WebHost
    .UseKestrelCore();

services
    .AddRoutingCore()
    .AddOpenApi("v1", opts => { opts.AddDocumentTransformer<BearerSecuritySchemaTransformer>(); })
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
    .AddCarter()
    .AddEnvironmentFlow(builder, configuration)
    .AddAuthenticationConfig(configuration)
    .AddMediatrHandlers()
    .AddExceptionMiddlewareSerializer()
    .AddHttpContextAccessor();

services.AddDbConfiguration(configuration);
services.AddSingleton<AuthenticationService>();

// TODO: should be env based
builder.WebHost.ConfigureKestrel((_, options) => { options.ListenAnyIP(5555); });

var app = builder.Build();

/* MIDDLEWARES */

app.MapOpenApi();
app.MapCarter();
app.UseCors("CorsFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionMiddlewareModule();

/* RUN */
var appDbContext = app.Services.GetRequiredService<ApplicationDbContext>();

await appDbContext.Database.EnsureDeletedAsync();
await appDbContext.Database.EnsureCreatedAsync();

app.Run();