using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Environment;
using BalancedBooksAPI.Core.Exceptions;
using BalancedBooksAPI.Core.Mediatr;
using BalancedBooksAPI.Core.OpenApi.DocumentTransformers;
using BalancedBooksAPI.Features.Authentication.Config;
using BalancedBooksAPI.Features.Authentication.Services;
using Carter;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateEmptyBuilder(new()
{
    EnvironmentName = "development"
});

var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost
    .UseKestrelCore();

services
    .AddRoutingCore()
    .AddEnvironmentFlow(builder, configuration)
    .AddOpenApi("v1", opts =>
    {
        // TODO: from typed config
        var url = Environment.GetEnvironmentVariable("HTTP__URL");
        
        opts.AddDocumentTransformer((document, _, _) =>
        {
            document.Servers = new List<OpenApiServer>
            {

                new() { Url = url }
            };

            return Task.CompletedTask;
        });
        opts.AddDocumentTransformer<BearerSecuritySchemaTransformer>();
    })
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