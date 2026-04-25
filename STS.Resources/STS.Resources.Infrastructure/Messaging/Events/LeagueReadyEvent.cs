namespace STS.Resources.Infrastructure.Messaging.Events;

public sealed record LeagueReadyEvent
{
    public string RedisKey { get; init; } = string.Empty;
}