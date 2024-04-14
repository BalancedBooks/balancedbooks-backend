using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace balancedbooks_backend.Core.OpenAPI;

public class SetNonNullableAsRequiredSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        var additionalRequiredProps = model.Properties
            .Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))
            .Select(x => x.Key);
        foreach (var propKey in additionalRequiredProps)
        {
            model.Required.Add(propKey);
        }
    }
}
