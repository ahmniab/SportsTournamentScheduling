using Authzed.Api.V1;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using STS.Resources.Infrastructure.Messaging;
using STS.Resources.Infrastructure.Persistence;
using STS.Resources.Infrastructure.Repositories;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.League.Commands.PrepareLeague;
using STS.Resources.Infrastructure.Messaging.Publishers;
using STS.Resources.Infrastructure.Messaging.Consumers;
using STS.Resources.Infrastructure.Services;

namespace STS.Resources.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableSensitiveDataLogging = false)
    {
        services.AddDbContext<ResourcesDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ResourcesDb") 
                                  ?? throw new NullReferenceException("No connection string for ResourcesDb"));
                // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.enablesensitivedatalogging?view=efcore-10.0
                if (enableSensitiveDataLogging)
                    options.EnableSensitiveDataLogging();
            });
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") 
                                          ?? throw new NullReferenceException("No Connection String for Redis")));

        var rabbitMqOptions = new RabbitMqOptions
        {
            HostName = configuration["RabbitMq:HostName"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest",
            VirtualHost = configuration["RabbitMq:VirtualHost"] ?? "/"
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
        services.AddAuthzedAPi(configuration);
        services.AddScoped<ILeaguePermissionService, LeaguePermissionService>();
        return services;
    }
    private static IServiceCollection AddAuthzedAPi(this IServiceCollection services, IConfiguration configuration)
    {
        var authEndPoint = configuration["AuthzedApi:Url"] 
                           ?? throw  new NullReferenceException("AuthzedApi:Url is null");
        var authToken = configuration["AuthzedApi:Token"] 
                        ?? throw new NullReferenceException("AuthzedApi:Token is null");
        
        var uri = new Uri(authEndPoint);
        var isHttps = uri.Scheme == "https";
        
        // Allow insecure connections for development (HTTP)
        if (!isHttps)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
        
        services.AddGrpcClient<PermissionsService.PermissionsServiceClient>(options =>
        {
            options.Address = uri;
        })
        .ConfigureChannel(options =>
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("Authorization", $"Bearer {authToken}");
                return Task.CompletedTask;
            });

            if (isHttps)
            {
                // HTTPS: Use SSL credentials with call credentials
                options.Credentials = ChannelCredentials.Create(new SslCredentials(), credentials);
            }
            else
            {
                // HTTP: Enable unsafe insecure channel with call credentials
                options.UnsafeUseInsecureChannelCallCredentials = true;
                options.Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials);
            }
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            if (isHttps)
            {
                // Bypass SSL certificate validation for self-signed certs in development
                handler.ServerCertificateCustomValidationCallback = 
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }
            return handler;
        });
        return services;
    }
}