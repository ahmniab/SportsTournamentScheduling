using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.Team;

namespace STS.Resources.Application.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(string leagueId);
    Task<Team> GetTeamByIdAsync(string id);
    Task<Team> CreateTeamAsync(CreateTeamCommand team);
    Task<Team> UpdateTeamAsync(UpdateTeamCommand team);
    Task DeleteTeamAsync(string id);
    Task<bool> VerifyOwnershipAsync(string leagueId, string teamId);
}