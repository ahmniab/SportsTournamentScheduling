using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Application.Messaging.Publishers;
using STS.TimeTables.Infrastructure.Messaging;
using STS.TimeTables.Infrastructure.Messaging.Consumers;
using STS.TimeTables.Infrastructure.Messaging.Publishers;
using STS.TimeTables.Infrastructure.Persistence;
using STS.TimeTables.Infrastructure.Repositories;

namespace STS.TimeTables.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableSensitiveDataLogging = false)
    {
        services.AddPostgres(configuration, enableSensitiveDataLogging);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        
        services.AddRabbitMq(configuration.GetSection("RabbitMq"));
        
        services.AddScoped<ILeagueRepository, LeagueRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ILeaguePreparePublisher, LeaguePreparePublisher>();

        services.AddHostedService<MatchesCompletedConsumer>();

        return services;
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfigurationSection rabbitMqSection)
    {
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

    public static IServiceCollection AddPostgres(
        this IServiceCollection services, 
        IConfiguration configuration,
        bool enableSensitiveDataLogging = false)
    {
        services.AddDbContext<TimeTableDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("TimeTableDb"));
            if (enableSensitiveDataLogging)
                options.EnableSensitiveDataLogging();
        });
        return services;
    }
}