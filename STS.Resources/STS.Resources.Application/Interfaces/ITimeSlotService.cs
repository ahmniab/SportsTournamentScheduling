using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.TimeSlot;

namespace STS.Resources.Application.Interfaces;

public interface ITimeSlotService
{
    Task<IEnumerable<TimeSlot>> GetTimeSlotsByLeagueIdAsync(string leagueId);
    Task<TimeSlot> GetTimeSlotByIdAsync(string id);
    Task<TimeSlot> CreateTimeSlotAsync(CreateTimeSlotCommand timeSlot);
    Task<TimeSlot> UpdateTimeSlotAsync(UpdateTimeSlotCommand timeSlot);
    Task DeleteTimeSlotAsync(string id);
}