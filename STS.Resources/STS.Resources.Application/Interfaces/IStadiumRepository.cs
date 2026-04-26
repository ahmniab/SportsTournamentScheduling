using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface IStadiumRepository
{
    Task<Stadium?> GetByIdAsync(Guid id);
    Task<IEnumerable<Stadium>?> GetByLeagueIdAsync(Guid leagueId);
    Task<int> GetCountByIdAndOwnerIdWithNoTracksAsync(Guid id, Guid ownerId);
    Task<Stadium> AddAsync(Stadium stadium);
    Task<Stadium> UpdateAsync(Stadium stadium);
    Task DeleteAsync(Guid id);
    Task DeleteStadiumAsyncByLeagueIdAsync(Guid leagueId);
}