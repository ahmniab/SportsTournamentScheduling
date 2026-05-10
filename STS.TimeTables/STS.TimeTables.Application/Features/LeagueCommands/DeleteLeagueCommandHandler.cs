using STS.TimeTables.Application.Interfaces;

namespace STS.TimeTables.Application.Features.LeagueCommands;

public class DeleteLeagueCommandHandler
{
    private readonly ILeagueRepository _leagueRepository;

    public DeleteLeagueCommandHandler(ILeagueRepository repository)
    {
        _leagueRepository = repository;
    }

    public async Task Handle(DeleteLeagueCommand command, CancellationToken ct = default)
    {
        if (!Guid.TryParse(command.LeagueId, out var id) || id == Guid.Empty)
            throw new ArgumentException("League id is not a valid non-empty GUID.", nameof(command.LeagueId));

        await _leagueRepository.DeleteLeagueAsync(id);
    }
}
