using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.Stadium;

namespace STS.Resources.Application.Interfaces;

public interface IStadiumService
{
    Task<IEnumerable<Stadium>> GetStadiumsByLeagueIdAsync(string leagueId);
    Task<Stadium> GetStadiumByIdAsync(string id);
    Task<Stadium> CreateStadiumAsync(CreateStadiumCommand stadium);
    Task<Stadium> UpdateStadiumAsync(UpdateStadiumCommand stadium);
    Task DeleteStadiumAsync(string id);
}