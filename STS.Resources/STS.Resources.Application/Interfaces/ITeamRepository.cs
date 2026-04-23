using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetTeamByIdAsync(Guid teamId);
    Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(Guid leagueId);
    Task AddTeamAsync(Team team);
    Task UpdateTeamAsync(Team team);
    Task DeleteTeamAsync(Guid teamId);
}