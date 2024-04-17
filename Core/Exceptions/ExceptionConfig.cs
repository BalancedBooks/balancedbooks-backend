namespace BalancedBooks_API.Core.Exceptions;

public static class ExceptionConfig
{
    /// <summary>
    /// Registers ExceptionSerializer middleware
    /// </summary>
    /// <returns></returns>
    public static IServiceCollection AddExceptionMiddlewareSerializer(this IServiceCollection services)
    {
        return services.AddTransient<ExceptionSerializerMiddleware>();
    }

    /// <summary>
    /// Add ExceptionSerializer to the ASP.NET Pipeline
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static void UseExceptionMiddlewareModule(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionSerializerMiddleware>();
    }
}