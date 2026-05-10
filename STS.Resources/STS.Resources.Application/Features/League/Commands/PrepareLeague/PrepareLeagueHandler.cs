using StackExchange.Redis;
using STS.Resources.Application.Extensions;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;
using STS.Resources.Application.Models.Responses;

namespace STS.Resources.Application.Features.League.Commands.PrepareLeague;

public sealed class PrepareLeagueHandler
{
    private readonly ILeagueService _leagueRepository;
    private readonly IDatabase _db;
    private readonly ILeagueReadyPublisher _leagueReadyPublisher;

    public PrepareLeagueHandler(
        ILeagueService leagueRepository,
        IConnectionMultiplexer redis,
        ILeagueReadyPublisher publisher)
    {
        _leagueRepository = leagueRepository;
        _db = redis.GetDatabase();
        _leagueReadyPublisher = publisher;
    }

    public async Task HandleAsync(PrepareLeagueCommand command, CancellationToken ct = default)
    {
        var leagueJob = await _db.GetLeagueJobAsync(command.LeagueId) 
                        ??  throw new InvalidOperationException($"Could not parse league job ID {command.LeagueId}");
        try
        {
            LeagueResponse league = await GetLeagueData(command.LeagueId);
            var redisKey = await _db.SetLeagueAsync(league, ct);
        
            leagueJob.Status = LeagueJobStatus.Prepared;
            await _db.SetLeagueJobAsync(leagueJob, ct);
        
            await _leagueReadyPublisher.PublishAsync(new PrepareLeagueResult
            {
                RedisKey = redisKey,
                LeagueId = command.LeagueId
            }, ct);
        }
        catch (Exception ex)
        {
            leagueJob.Status = LeagueJobStatus.Failed;
            leagueJob.ErrorMessage = ex.Message;
            
            await _db.SetLeagueJobAsync(leagueJob, ct);
        }
        
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
        if (league.Teams != null && league.Teams.Count() <= 2)
        {
            throw new InvalidOperationException("You do not have enough teams for this league");
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
