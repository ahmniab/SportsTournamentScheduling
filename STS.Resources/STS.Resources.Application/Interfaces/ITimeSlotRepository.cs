using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetByIdAsync(Guid id);
    Task<IEnumerable<TimeSlot>?> GetByLeagueIdAsync(Guid leagueId);
    Task<TimeSlot> AddAsync(TimeSlot timeSlot);
    Task<TimeSlot> UpdateAsync(TimeSlot timeSlot);
    Task DeleteAsync(Guid id);
    Task DeleteTimeSlotAsyncByLeagueIdAsync(Guid leagueId);
}