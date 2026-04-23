using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.Application.Services;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;

    public LeagueService(ILeagueRepository leagueRepository, ITeamRepository teamRepository)
    {
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
    }

    public async Task<League?> GetLeagueByIdAsync(Guid id)
    {
        return await _leagueRepository.GetByIdAsync(id);
    }

    public async Task<List<League>?> GetLeaguesByOwnerIdAsync(Guid ownerId)
    {
        return await _leagueRepository.GetByOwnerIdAsync(ownerId);
    }

    public async Task CreateLeagueAsync(League league)
    {
        await _leagueRepository.AddAsync(league);
    }

    public async Task UpdateLeagueAsync(League league)
    {
        await _leagueRepository.UpdateAsync(league);
    }

    public async Task DeleteLeagueAsync(Guid Id)
    {
        await _leagueRepository.GetByIdAsync(Id);
        await _leagueRepository.DeleteAsync(Id);
    }
}