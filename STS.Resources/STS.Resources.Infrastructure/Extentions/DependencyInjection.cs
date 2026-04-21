using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ResourcesDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}