using System.Globalization;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.TimeSlot;
using STS.Resources.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Application.Services;

public class TimeSlotService : ITimeSlotService
{
    private const int MaxTimeSlotsPerLeague = 3;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public TimeSlotService(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<TimeSlot> GetTimeSlotByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var timeSlotGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }

        return await _timeSlotRepository.GetByIdAsync(timeSlotGuid) ?? throw new KeyNotFoundException("Time slot was not found.");
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByLeagueIdAsync(string leagueId)
    {
        if (!Guid.TryParse(leagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(leagueId));
        }

        var timeSlots = await _timeSlotRepository.GetByLeagueIdAsync(leagueGuid);
        return timeSlots ?? throw new KeyNotFoundException("No time slots were found for the requested league.");
    }

    public async Task<TimeSlot> CreateTimeSlotAsync(CreateTimeSlotCommand timeSlot)
    {
        if (!Guid.TryParse(timeSlot.LeagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(timeSlot.LeagueId));
        }

        var newTimeSlot = new TimeSlot
        {
            LeagueId = leagueGuid,
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime
        };

        ValidateTimeSlot(newTimeSlot);

        var existingTimeSlots = await _timeSlotRepository.GetByLeagueIdAsync(newTimeSlot.LeagueId) ?? Enumerable.Empty<TimeSlot>();
        if (existingTimeSlots.Count() >= MaxTimeSlotsPerLeague)
        {
            throw new InvalidOperationException($"A league can have at most {MaxTimeSlotsPerLeague} time slots.");
        }

        return await _timeSlotRepository.AddAsync(newTimeSlot);
    }

    public async Task<TimeSlot> UpdateTimeSlotAsync(UpdateTimeSlotCommand timeSlot)
    {
        if (!Guid.TryParse(timeSlot.Id, out var timeSlotGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(timeSlot.Id));
        }

        try
        {
            var updatedTimeSlot = new TimeSlot
            {
                Id = timeSlotGuid,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime
            };

            ValidateTimeSlot(updatedTimeSlot,  false);
            await _timeSlotRepository.UpdateAsync(updatedTimeSlot);
            return updatedTimeSlot;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new KeyNotFoundException("Time slot was not found or has been deleted.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Time slot was not found.", ex);
        }
    }

    public async Task DeleteTimeSlotAsync(string id)
    {
        if (!Guid.TryParse(id, out var timeSlotGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }

        await _timeSlotRepository.DeleteAsync(timeSlotGuid);
    }

    private static void ValidateTimeSlot(TimeSlot timeSlot, bool validateLeagueId = true)
    {
        if (validateLeagueId)
            if (timeSlot.LeagueId == Guid.Empty)
                throw new ArgumentException("league_id must be a valid GUID.");
            

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