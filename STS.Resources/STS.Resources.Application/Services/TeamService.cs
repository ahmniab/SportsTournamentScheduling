using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<Team?> GetTeamByIdAsync(Guid teamId)
    {
        return await _teamRepository.GetTeamByIdAsync(teamId);
    }

    public async Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(Guid leagueId)
    {
        return await _teamRepository.GetTeamsByLeagueIdAsync(leagueId);
    }

    public async Task AddTeamAsync(Team team)
    {
        await _teamRepository.AddTeamAsync(team);
    }

    public async Task UpdateTeamAsync(Team team)
    {
        await _teamRepository.UpdateTeamAsync(team);
    }

    public async Task DeleteTeamAsync(Guid teamId)
    {
        await _teamRepository.DeleteTeamAsync(teamId);
    }
}