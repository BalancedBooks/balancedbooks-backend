using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace BalancedBooksAPI.Core.OpenApi.DocumentTransformers;

public class BearerSecuritySchemaTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    private const string SecuritySchemaId = "Bearer";

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == SecuritySchemaId))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                [SecuritySchemaId] = new()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", //
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }


        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
        {
            // if operation is annotated with 401 status code (e.g Produces<Type>(401)
            if (operation.Value.Responses.Any(r => r.Key == "401"))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = SecuritySchemaId,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
        }
    }
}