using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.League;
namespace STS.Resources.Application.Interfaces;

public interface ILeagueService
{
    Task<IEnumerable<League>> GetLeaguesByOwnerIdAsync(String ownerId);
    Task<League> GetLeagueByIdAsync(String id);
    Task<League> CreateLeagueAsync(CreateLeagueCommand league);
    Task<League> UpdateLeagueAsync(UpdateLeagueCommand league);
    Task DeleteLeagueAsync(String Id);
}