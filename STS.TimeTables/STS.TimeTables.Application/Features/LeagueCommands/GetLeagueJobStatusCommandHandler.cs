using StackExchange.Redis;
using STS.TimeTables.Application.Extensions.LeagueJobExtensions;
using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Features.LeagueCommands;

public class GetLeagueJobStatusCommandHandler
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILeagueRepository _leagueRepository;

    public GetLeagueJobStatusCommandHandler(
        IConnectionMultiplexer connectionMultiplexer,
        ILeagueRepository leagueRepository)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _leagueRepository = leagueRepository;
    }

    public async Task<LeagueJob> Handle(GetLeagueJobStatusCommand command, CancellationToken ct = default)
    {
        if (!Guid.TryParse(command.LeagueId, out var leagueId) || leagueId == Guid.Empty)
            throw new ArgumentException("League id is not a valid non-empty GUID.", nameof(command.LeagueId));

        var db = _connectionMultiplexer.GetDatabase();
        var leagueJob = await db.GetLeagueJobAsync(leagueId, ct);

        if (leagueJob is not null)
            return leagueJob;

        var league = await _leagueRepository.GetLeagueSummaryAsync(leagueId);
        if (league is null)
            throw new KeyNotFoundException($"League job for league '{leagueId}' was not found.");

        return new LeagueJob
        {
            LeagueId = leagueId,
            Status = LeagueJobStatus.Completed,
            // Intentionally leave CreatedAt, UpdatedAt, ErrorMessage as defaults
        };
    }
}
