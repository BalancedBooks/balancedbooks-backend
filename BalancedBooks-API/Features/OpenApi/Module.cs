using BalancedBooksAPI.Core;
using Carter;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Features.OpenApi;

public class OpenApiModule : CarterModule
{
    public OpenApiModule() : base("/openapi")
    {
      
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (IOptions<HttpConfig> options) =>
        {
            var schemaUrl = $"{options.Value.Url}/swagger/v1/swagger.json";

            return Results.Content($"""
                                      <html>
                                      <head>
                                        <title>API Reference</title>
                                        <meta charset="utf-8" />
                                        <meta
                                          name="viewport"
                                          content="width=device-width, initial-scale=1" />
                                      </head>
                                      <body>
                                        <script
                                          id="api-reference"
                                          data-url="{schemaUrl}"
                                          ></script>
                                        <!-- You can also set a full configuration object like this -->
                                        <!-- easier for nested objects -->
                                        <script>
                                          var configuration = ""
                                          var apiReference = document.getElementById('api-reference')
                                          apiReference.dataset.configuration = JSON.stringify(configuration)
                                        </script>
                                        <script src="https://cdn.jsdelivr.net/npm/@scalar/api-reference"></script>
                                      </body>
                                    </html>
                                    """, "text/html");
        }).ExcludeFromDescription();
    }
}