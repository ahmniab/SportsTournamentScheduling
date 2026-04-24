using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.League;
namespace STS.Resources.Application.Interfaces;

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    public Task<League?> GetByIdAsync(
        Guid id, 
        bool includeTeams = false,  
        bool includeStadiums = false,
        bool includeTimeSlots = false
    );
    
    Task<IEnumerable<League>?> GetByOwnerIdAsync(Guid ownerId);
    Task<League> AddAsync(League league);
    Task<League> UpdateAsync(League league);
    Task DeleteAsync(Guid Id);
}