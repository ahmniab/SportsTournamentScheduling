using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.Application.Services;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public LeagueService(ILeagueRepository leagueRepository, ITeamRepository teamRepository, IStadiumRepository stadiumRepository, ITimeSlotRepository timeSlotRepository)
    {
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _timeSlotRepository = timeSlotRepository;
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
        await _teamRepository.DeleteTeamAsyncByLeagueIdAsync(Id);
        await _stadiumRepository.DeleteStadiumAsyncByLeagueIdAsync(Id);
        await _timeSlotRepository.DeleteTimeSlotAsyncByLeagueIdAsync(Id);
        await _leagueRepository.DeleteAsync(Id);
    }
}