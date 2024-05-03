using CommunityToolkit.Diagnostics;
using Microsoft.OpenApi.Models;

namespace BalancedBooks_API.Core.OpenAPI;

public static class SwaggerConfig
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddEndpointsApiExplorer();

        services.AddHttpConfig();

        var httpConfig = configurationManager.GetRequiredSection(HttpConfig.ConfigKey).Get<HttpConfig>();

        Guard.IsNotNull(httpConfig);

        services.AddSwaggerGen(opts =>
        {
            opts.SchemaFilter<SetNonNullableAsRequiredSchemaFilter>();

            opts.DescribeAllParametersInCamelCase();
            opts.SupportNonNullableReferenceTypes();

            opts.AddServer(new OpenApiServer
            {
                Url = httpConfig.Url
            });

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Balanced Books - API",
                Version = "v1",
                Description = "",
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDependencies(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts => { opts.SwaggerEndpoint("v1/swagger.json", "Backend schema"); });

        return app;
    }
}