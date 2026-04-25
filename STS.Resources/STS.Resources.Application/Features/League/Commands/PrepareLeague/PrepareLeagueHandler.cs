using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.Application.Features.League.Commands.PrepareLeague;

public sealed class PrepareLeagueHandler
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IDistributedCache _cache;
    private readonly ILeagueReadyPublisher _publisher;

    public PrepareLeagueHandler(
        ILeagueRepository leagueRepository,
        IDistributedCache cache,
        ILeagueReadyPublisher publisher)
    {
        _leagueRepository = leagueRepository;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task HandleAsync(PrepareLeagueCommand command, CancellationToken ct = default)
    {
        var league = await _leagueRepository.GetByIdAsync(
            command.LeagueId,
            includeTeams: true,
            includeStadiums: true,
            includeTimeSlots: true
        ) ?? throw new KeyNotFoundException($"League {command.LeagueId} was not found.");

        var redisKey = $"league:prepared:{command.LeagueId}";

        var serialized = JsonSerializer.Serialize(league);

        await _cache.SetStringAsync(
            redisKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            },
            ct
        );

        await _publisher.PublishAsync(new PrepareLeagueResult
        {
            RedisKey = redisKey
        }, ct);
    }
}