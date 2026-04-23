using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface IStadiumService
{
    Task<Stadium?> GetStadiumByIdAsync(Guid stadiumId);
    Task<IEnumerable<Stadium>> GetStadiumsByLeagueIdAsync(Guid leagueId);
    Task AddStadiumAsync(Stadium stadium);
    Task UpdateStadiumAsync(Stadium stadium);
    Task DeleteStadiumAsync(Guid stadiumId);
}