using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using STS.Resources.Infrastructure.Messaging;
using STS.Resources.Infrastructure.Persistence;
using STS.Resources.Infrastructure.Repositories;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.League.Commands.PrepareLeague;
using STS.Resources.Infrastructure.Messaging.Publishers;
using STS.Resources.Infrastructure.Messaging.Consumers;

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
        services.AddScoped<ILeagueRepository, LeagueRepository>();

        services.AddScoped<PrepareLeagueHandler>();
        services.AddScoped<ILeagueReadyPublisher, LeagueReadyPublisher>();
        services.AddHostedService<LeaguePrepareConsumer>();
        return services;
    }
}