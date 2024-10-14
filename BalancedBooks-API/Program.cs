using BalancedBooks_Integrations_CompanyRegistry;
using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Db.Casbin;
using BalancedBooksAPI.Core.Environment;
using BalancedBooksAPI.Core.Exceptions;
using BalancedBooksAPI.Core.Mediatr;
using BalancedBooksAPI.Core.OpenAPI;
using BalancedBooksAPI.Features.Authentication.Config;
using BalancedBooksAPI.Features.Authentication.Services;
using Carter;
using CommunityToolkit.Diagnostics;

var opts = new WebApplicationOptions()
{
    EnvironmentName = "development"
};

var builder = WebApplication.CreateEmptyBuilder(opts);

var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost
    .UseKestrelCore();

services
    .AddRoutingCore()
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
    .AddOpenApiDocumentation(configuration)
    .AddMediatrHandlers()
    .AddExceptionMiddlewareSerializer()
    .AddHttpContextAccessor();

services.AddDbConfiguration(configuration);
services.AddSingleton<AuthenticationService>();

// TODO: should be env based
builder.WebHost.ConfigureKestrel((_, options) => { options.ListenAnyIP(5555); });

var app = builder.Build();

/* MIDDLEWARES */

app.MapCarter();
app.UseSwaggerDependencies();
app.UseCors("CorsFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionMiddlewareModule();

/* RUN */
var appDbContext = app.Services.GetRequiredService<ApplicationDbContext>();
var casbinDbContext = app.Services.GetRequiredService<AppCasbinDbContext>();

await appDbContext.Database.EnsureDeletedAsync();
await appDbContext.Database.EnsureCreatedAsync();
await casbinDbContext.Database.EnsureCreatedAsync();

app.Run();