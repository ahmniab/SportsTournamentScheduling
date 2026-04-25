using System.Text.Json;
using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Models.Responses;

namespace STS.Resources.Application.Features.League.Commands.PrepareLeague;

public sealed class PrepareLeagueHandler
{
    private readonly ILeagueService _leagueRepository;
    private readonly IDistributedCache _cache;
    private readonly ILeagueReadyPublisher _publisher;

    public PrepareLeagueHandler(
        ILeagueService leagueRepository,
        IDistributedCache cache,
        ILeagueReadyPublisher publisher)
    {
        _leagueRepository = leagueRepository;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task HandleAsync(PrepareLeagueCommand command, CancellationToken ct = default)
    {
        var league = await GetLeagueData(command.LeagueId);
        
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

    private async Task<LeagueResponse> GetLeagueData(Guid leagueId)
    {
        GetLeagueByIdCommand cmd = new GetLeagueByIdCommand
        {
            Id = leagueId.ToString(),
            IncludeOptions = new LeagueIncludeOptions
            {
                IncludeStadiums = IncludeOption.INCLUDE_ID,
                IncludeTeams = IncludeOption.INCLUDE_ID,
                IncludeTimeSlots =  IncludeOption.INCLUDE_ID
            }
        };
        
        var league = await _leagueRepository.GetLeagueByIdAsync(cmd);
        if (league.Teams != null && !league.Teams.Any())
        {
            throw new InvalidOperationException("You do not have any teams for this league");
        }

        if (league.Stadiums != null && !league.Stadiums.Any())
        {
            throw new InvalidOperationException("You do not have any stadiums for this league");
        }

        if (league.TimeSlots != null && !league.TimeSlots.Any())
        {
            throw new InvalidOperationException("You do not have any time slots for this league");
        }

        return league;

    }
}