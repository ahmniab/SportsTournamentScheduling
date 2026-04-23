using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    Task<IEnumerable<League>?> GetByOwnerIdAsync(Guid ownerId);
    Task<League> AddAsync(League league);
    Task<League> UpdateAsync(League league);
    Task DeleteAsync(Guid Id);
}