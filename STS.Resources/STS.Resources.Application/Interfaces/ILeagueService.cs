using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueService
{
    Task<List<League>?> GetLeaguesByOwnerIdAsync(Guid ownerId);
    Task<League?> GetLeagueByIdAsync(Guid id);
    Task CreateLeagueAsync(League league);
    Task UpdateLeagueAsync(League league);
    Task DeleteLeagueAsync(Guid leagueId);
}