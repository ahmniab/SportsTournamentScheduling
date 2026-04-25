using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using STS.Resources.Infrastructure.Messaging;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string psqlConnectionString,
        string redisConnectionString,
        IConfigurationSection rabbitMqSection,
        bool enableSensitiveDataLogging = false)
    {
        services.AddDbContext<ResourcesDbContext>(options =>
            {
                options.UseNpgsql(psqlConnectionString);
                // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.enablesensitivedatalogging?view=efcore-10.0
                if (enableSensitiveDataLogging)
                    options.EnableSensitiveDataLogging();
            });
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        var rabbitMqOptions = new RabbitMqOptions
        {
            HostName = rabbitMqSection["HostName"] ?? "localhost",
            Port = int.TryParse(rabbitMqSection["Port"], out var parsedPort) ? parsedPort : 5672,
            UserName = rabbitMqSection["UserName"] ?? "guest",
            Password = rabbitMqSection["Password"] ?? "guest",
            VirtualHost = rabbitMqSection["VirtualHost"] ?? "/"
        };

        services.AddSingleton(rabbitMqOptions);
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            HostName = rabbitMqOptions.HostName,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.UserName,
            Password = rabbitMqOptions.Password,
            VirtualHost = rabbitMqOptions.VirtualHost
        });

        return services;
    }
}