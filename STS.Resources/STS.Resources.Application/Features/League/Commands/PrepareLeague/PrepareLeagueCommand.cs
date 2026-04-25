namespace STS.Resources.Application.Features.League.Commands.PrepareLeague;

public sealed record PrepareLeagueCommand
{
    public Guid LeagueId { get; init; }
}