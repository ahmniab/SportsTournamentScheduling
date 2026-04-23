using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueService
{
    Task<IEnumerable<League>> GetLeaguesByOwnerIdAsync(Guid ownerId);
    Task<League?> GetLeagueByIdAsync(Guid id);
    Task CreateLeagueAsync(League league);
    Task UpdateLeagueAsync(League league);
    Task DeleteLeagueAsync(Guid Id);
}