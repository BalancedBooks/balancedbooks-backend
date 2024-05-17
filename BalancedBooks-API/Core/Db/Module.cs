using BalancedBooksAPI.Core.Db.Casbin;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooksAPI.Core.Db;

public class DbConfiguration
{
    public const string ConfigKey = "DB";

    [ConfigurationKeyName("CONNECTION_STRING")]
    public required string ConnectionString { get; init; }
}

public static class DbModule
{
    public static void AddDbConfiguration(this IServiceCollection service, IConfigurationManager configuration)
    {
        var config = configuration.GetSection(DbConfiguration.ConfigKey).Get<DbConfiguration>();

        Guard.IsNotNull(config);

        service.AddOptions<DbConfiguration>().BindConfiguration(DbConfiguration.ConfigKey);

        service.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(config.ConnectionString); });
        
        service.AddDbContext<AppCasbinDbContext>(builder =>
        {
            builder.UseNpgsql(config.ConnectionString);
        });
    }
}