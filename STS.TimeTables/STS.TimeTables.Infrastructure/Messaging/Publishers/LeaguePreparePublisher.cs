using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using STS.TimeTables.Application.Messaging.Publishers;
using STS.TimeTables.Infrastructure.Messaging.Events;

namespace STS.TimeTables.Infrastructure.Messaging.Publishers;

public class LeaguePreparePublisher : ILeaguePreparePublisher
{
    private readonly IConnectionFactory _connectionFactory;
    
    private readonly string QueueName =  "league.prepare";
    public LeaguePreparePublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task PublishAsync(Guid leagueId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,       
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct
        );

        var leaguePrepareEvent = new LeaguePrepareEvent { LeagueId = leagueId };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(leaguePrepareEvent)); 
        var props = new BasicProperties
        {
            Persistent = true,        // message survives broker restart
            ContentType = "application/json"
        };
        
        await channel.BasicPublishAsync(
            exchange: string.Empty,   
            routingKey: QueueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct
        );
    }
}