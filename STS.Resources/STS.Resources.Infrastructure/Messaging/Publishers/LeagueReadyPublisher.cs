using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using STS.Resources.Application.Features.League.Commands.PrepareLeague;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Messaging.Events;

namespace STS.Resources.Infrastructure.Messaging.Publishers;

public sealed class LeagueReadyPublisher : ILeagueReadyPublisher
{
    private readonly IConnectionFactory _connectionFactory;
    // Queue name the Matches Generator listens on
    private const string QueueName = "matches.generator";

    public LeagueReadyPublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task PublishAsync(PrepareLeagueResult result, CancellationToken ct = default)
    {
        // IConnection and IChannel are IAsyncDisposable in RabbitMQ.Client v7+
        await using var connection = await _connectionFactory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,       // survives broker restart
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct
        );

        var @event = new LeagueReadyEvent
        {
            RedisKey = result.RedisKey
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var props = new BasicProperties
        {
            Persistent = true,        // message survives broker restart
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,   // default exchange, routes by queue name
            routingKey: QueueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct
        );
    }
}