using System.Globalization;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Services;

public class TimeSlotService : ITimeSlotService
{
    private const int MaxTimeSlotsPerLeague = 3;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public TimeSlotService(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId)
    {
        return await _timeSlotRepository.GetTimeSlotByIdAsync(timeSlotId);
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByLeagueIdAsync(Guid leagueId)
    {
        return await _timeSlotRepository.GetTimeSlotsByLeagueIdAsync(leagueId);
    }

    public async Task AddTimeSlotAsync(TimeSlot timeSlot)
    {
        ValidateTimeSlot(timeSlot);

        var existingTimeSlots = await _timeSlotRepository.GetTimeSlotsByLeagueIdAsync(timeSlot.LeagueId);
        if (existingTimeSlots.Count() >= MaxTimeSlotsPerLeague)
        {
            throw new InvalidOperationException($"A league can have at most {MaxTimeSlotsPerLeague} time slots.");
        }

        await _timeSlotRepository.AddTimeSlotAsync(timeSlot);
    }

    public async Task UpdateTimeSlotAsync(TimeSlot timeSlot)
    {
        ValidateTimeSlot(timeSlot);
        await _timeSlotRepository.UpdateTimeSlotAsync(timeSlot);
    }

    public async Task DeleteTimeSlotAsync(Guid timeSlotId)
    {
        await _timeSlotRepository.DeleteTimeSlotAsync(timeSlotId);
    }

    private static void ValidateTimeSlot(TimeSlot timeSlot)
    {
        if (timeSlot.LeagueId == Guid.Empty)
        {
            throw new ArgumentException("league_id must be a valid GUID.");
        }

        var startTime = NormalizeAndValidateTime(timeSlot.StartTime, nameof(timeSlot.StartTime));
        var endTime = NormalizeAndValidateTime(timeSlot.EndTime, nameof(timeSlot.EndTime));

        if (endTime <= startTime)
        {
            throw new ArgumentException("end_time must be greater than start_time.");
        }

        timeSlot.StartTime = startTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        timeSlot.EndTime = endTime.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    private static TimeOnly NormalizeAndValidateTime(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }

        if (!DateTime.TryParseExact(
                value.Trim(),
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedTime))
        {
            throw new ArgumentException($"{fieldName} must be in 24-hour HH:mm format.");
        }

        return TimeOnly.FromDateTime(parsedTime);
    }
}