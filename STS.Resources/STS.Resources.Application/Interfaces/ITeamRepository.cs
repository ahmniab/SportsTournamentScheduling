using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>?> GetByLeagueIdAsync(Guid leagueId);
    Task<Team> AddAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
    Task DeleteTeamAsyncByLeagueIdAsync(Guid leagueId);
}