using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository stadiumRepository)
    {
        _stadiumRepository = stadiumRepository;
    }

    public async Task<Stadium?> GetStadiumByIdAsync(Guid stadiumId)
    {
        return await _stadiumRepository.GetStadiumByIdAsync(stadiumId);
    }

    public async Task<IEnumerable<Stadium>> GetStadiumsByLeagueIdAsync(Guid leagueId)
    {
        return await _stadiumRepository.GetStadiumsByLeagueIdAsync(leagueId);
    }

    public async Task AddStadiumAsync(Stadium stadium)
    {
        await _stadiumRepository.AddStadiumAsync(stadium);
    }

    public async Task UpdateStadiumAsync(Stadium stadium)
    {
        await _stadiumRepository.UpdateStadiumAsync(stadium);
    }

    public async Task DeleteStadiumAsync(Guid stadiumId)
    {
        await _stadiumRepository.DeleteStadiumAsync(stadiumId);
    }
}