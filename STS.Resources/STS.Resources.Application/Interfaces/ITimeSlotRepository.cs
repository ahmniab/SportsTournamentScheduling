using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId);
    Task<IEnumerable<TimeSlot>> GetTimeSlotsByLeagueIdAsync(Guid leagueId);
    Task AddTimeSlotAsync(TimeSlot timeSlot);
    Task UpdateTimeSlotAsync(TimeSlot timeSlot);
    Task DeleteTimeSlotAsync(Guid timeSlotId);
    Task DeleteTimeSlotAsyncByLeagueIdAsync(Guid leagueId);
}