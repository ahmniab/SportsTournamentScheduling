using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Interfaces;

public interface IMatchRepository
{
    public Task<IEnumerable<Match>> GetMatchesAsync(Guid leagueId);
    public Task<Match?> GetMatchAsync(Guid id);
}