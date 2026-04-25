namespace STS.Resources.Infrastructure.Messaging.Events;

public sealed record LeaguePrepareEvent
{
    public Guid LeagueId { get; init; }
}