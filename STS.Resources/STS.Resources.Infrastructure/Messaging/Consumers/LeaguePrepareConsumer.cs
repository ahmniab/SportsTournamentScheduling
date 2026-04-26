using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using STS.Resources.Application.Features.League.Commands.PrepareLeague;
using STS.Resources.Infrastructure.Messaging.Events;

namespace STS.Resources.Infrastructure.Messaging.Consumers;

public sealed class LeaguePrepareConsumer : BackgroundService
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LeaguePrepareConsumer> _logger;
    private const string QueueName = "league.prepare";

    public LeaguePrepareConsumer(
        IConnectionFactory connectionFactory,
        IServiceScopeFactory scopeFactory, 
        ILogger<LeaguePrepareConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );
        
        await channel.BasicQosAsync(
            prefetchSize: 0, 
            prefetchCount: 1, 
            global: false, 
            cancellationToken: stoppingToken
            );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.Span);

            try
            {
                var @event = JsonSerializer.Deserialize<LeaguePrepareEvent>(body)
                    ?? throw new InvalidOperationException("Failed to deserialize LeaguePrepareEvent.");
                
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<PrepareLeagueHandler>();

                await handler.HandleAsync(new PrepareLeagueCommand
                {
                    LeagueId = @event.LeagueId
                }, stoppingToken);
                
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                _logger.LogInformation("league.prepare processed for LeagueId={LeagueId}", @event.LeagueId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process league.prepare message. Body={Body}", body);
                
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,   
            consumer: consumer,
            cancellationToken: stoppingToken
        );
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}