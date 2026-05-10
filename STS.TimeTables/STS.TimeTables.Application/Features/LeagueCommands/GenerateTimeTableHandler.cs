using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using STS.TimeTables.Application.Extensions.LeagueJobExtensions;
using STS.TimeTables.Application.Messaging.Publishers;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Features.LeagueCommands;

public class GenerateTimeTableHandler
{
    private readonly IDatabase _db;
    private readonly ILeaguePreparePublisher _leaguePreparePublisher;
    private readonly ILogger<GenerateTimeTableHandler> _logger;

    public GenerateTimeTableHandler(
        IConnectionMultiplexer redis, 
        ILeaguePreparePublisher leaguePreparePublisher,
        ILogger<GenerateTimeTableHandler> logger)
    {
        _db = redis.GetDatabase();
        _leaguePreparePublisher = leaguePreparePublisher;
        _logger = logger;
    }

    public async Task HandleAsync(GenerateTimeTableCommand request, CancellationToken ct = default)
    {
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
            throw new ArgumentException($"League Id {request.LeagueId} is not a valid league id");

        try
        {
            var league = await _db.GetLeagueJobAsync(leagueId, ct);
            if (league != null 
                && (league.Status != LeagueJobStatus.Failed || league.Status != LeagueJobStatus.Completed))
            {
                _logger.LogInformation($"League {request.LeagueId} is being created");
                throw new InvalidOperationException($"League {leagueId} is already created");
            }
            
            await _db.SetLeagueJobAsync(new LeagueJob
                {
                    LeagueId = leagueId,
                    CreatedAt = DateTime.UtcNow,
                    Status = LeagueJobStatus.Created,
                },
                ct);
            await _leaguePreparePublisher.PublishAsync(leagueId, ct);
            _logger.LogInformation($"League {request.LeagueId} has been created");
        }
        catch (InvalidOperationException e)
        {
            throw;
        }
        catch (Exception e)
        {
            
        }
    }


}
