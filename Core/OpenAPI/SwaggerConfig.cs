using Microsoft.OpenApi.Models;

namespace balancedbooks_backend.Core.OpenAPI;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerDeps(this IServiceCollection services, HttpConfig httpConfig)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opts =>
        {
            opts.SchemaFilter<SetNonNullableAsRequiredSchemaFilter>();
            opts.SupportNonNullableReferenceTypes();
            opts.AddServer(new OpenApiServer()
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

    public static IApplicationBuilder UseSwaggerDeps(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts => { opts.SwaggerEndpoint("v1/swagger.json", "Backend schema"); });

        return app;
    }
}