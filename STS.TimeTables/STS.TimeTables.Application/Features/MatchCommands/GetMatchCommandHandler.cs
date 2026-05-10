using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Features.MatchCommands;

public class GetMatchCommandHandler
{
    private readonly IMatchRepository _matchRepository;

    public GetMatchCommandHandler(IMatchRepository repository)
    {
        _matchRepository = repository;
    }

    public async Task<Match> Handle(GetMatchCommand command, CancellationToken ct = default)
    {
        if (!Guid.TryParse(command.MatchId, out var id) || id == Guid.Empty)
            throw new ArgumentException("Match id is not a valid non-empty GUID.", nameof(command.MatchId));

        return await _matchRepository.GetMatchAsync(id)
               ?? throw new KeyNotFoundException($"Match '{id}' was not found.");
    }
}
