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
    private const string EventName = "league.prepared";

    public LeagueReadyPublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task PublishAsync(PrepareLeagueResult result, CancellationToken ct = default)
    {
        await PushToQueueAsync(result, ct);
    }

    private async Task PushToQueueAsync(PrepareLeagueResult result, CancellationToken ct = default)
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

        var readyEvent = new LeagueReadyEvent
        {
            RedisKey = result.RedisKey,
            LeagueId = result.LeagueId
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(readyEvent));

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

    private async Task PushToTopicEventAsync(PrepareLeagueResult result, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        await channel.ExchangeDeclareAsync(
            exchange: EventName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct
        );

        var readyEvent = new LeagueReadyEvent
        {
            RedisKey = result.RedisKey,
            LeagueId = result.LeagueId
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(readyEvent));

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: EventName,
            routingKey: EventName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct
        );
    }
}