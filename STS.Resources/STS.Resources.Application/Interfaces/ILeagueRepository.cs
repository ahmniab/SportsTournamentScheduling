using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    Task<List<League>?> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(League league);
    Task UpdateAsync(League league);
    Task DeleteAsync(Guid Id);
}