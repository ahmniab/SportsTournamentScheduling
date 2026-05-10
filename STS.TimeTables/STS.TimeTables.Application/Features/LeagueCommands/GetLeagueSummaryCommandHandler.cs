using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Features.LeagueCommands;

public class GetLeagueSummaryCommandHandler
{
    private readonly ILeagueRepository _leagueRepository;

    public GetLeagueSummaryCommandHandler(ILeagueRepository repository)
    {
        _leagueRepository = repository;
    }

    public async Task<League> Handle(GetLeagueSummaryCommand command, CancellationToken ct = default)
    {
        if (!Guid.TryParse(command.LeagueId, out var id) || id == Guid.Empty)
            throw new ArgumentException("League id is not a valid non-empty GUID.", nameof(command.LeagueId));

        return await _leagueRepository.GetLeagueSummaryAsync(id)
               ?? throw new KeyNotFoundException($"League '{id}' was not found.");
    }
}