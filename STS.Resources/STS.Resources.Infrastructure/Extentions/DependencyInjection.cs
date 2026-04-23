using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        bool enableSensitiveDataLogging = false)
    {
        services.AddDbContext<ResourcesDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.enablesensitivedatalogging?view=efcore-10.0
                if (enableSensitiveDataLogging)
                    options.EnableSensitiveDataLogging();
            });

        return services;
    }
}