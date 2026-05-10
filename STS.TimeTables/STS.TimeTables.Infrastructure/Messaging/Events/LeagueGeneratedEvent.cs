namespace STS.TimeTables.Infrastructure.Messaging.Events;

public class LeagueGeneratedEvent
{
    public Guid LeagueId { get; init; }
    public string LeagueJobId { get; init; } = string.Empty;
}