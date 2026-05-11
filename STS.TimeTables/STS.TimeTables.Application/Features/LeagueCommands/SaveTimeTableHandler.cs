using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using STS.TimeTables.Application.Extensions.LeagueExtensions;
using STS.TimeTables.Application.Extensions.LeagueJobExtensions;
using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Features.LeagueCommands;

public class SaveTimeTableHandler
{
    private readonly ILogger<SaveTimeTableHandler> _logger;
    private readonly ILeagueRepository _leagueRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IDatabase _redisDb;

    public SaveTimeTableHandler(
        ILogger<SaveTimeTableHandler> logger,
        ILeagueRepository leagueRepository,
        IMatchRepository matchRepository,
        IConnectionMultiplexer redis)
    {
        _logger = logger;
        _leagueRepository = leagueRepository;
        _matchRepository = matchRepository;
        _redisDb = redis.GetDatabase();
    }
    
    public async Task HandleAsync(SaveTimeTableCommand cmd, CancellationToken ct = default)
    {
        var league = await _redisDb.GetScheduledLeagueAsync(cmd.LeagueId);
        LeagueJob? leagueJob;

        leagueJob = await _redisDb.GetLeagueJobAsync(cmd.LeagueId, ct);

        if (league == null || leagueJob == null)
        {
            _logger.LogError("League {CmdLeagueId} not found", cmd.LeagueId);
            return;
        }
        await _leagueRepository.CreateLeagueAsync(league);
        leagueJob.Status = LeagueJobStatus.Completed;
        await _redisDb.SetLeagueJobAsync(leagueJob, ct);
    }
}