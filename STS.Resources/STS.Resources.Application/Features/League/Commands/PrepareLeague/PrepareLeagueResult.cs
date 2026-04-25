namespace STS.Resources.Application.Features.League.Commands.PrepareLeague;

public sealed record PrepareLeagueResult
{
    public string RedisKey { get; init; } = string.Empty;
}