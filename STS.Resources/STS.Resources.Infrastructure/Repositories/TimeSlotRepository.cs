using Microsoft.EntityFrameworkCore;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.Infrastructure.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly ResourcesDbContext _context;

    public TimeSlotRepository(ResourcesDbContext context)
    {
        _context = context;
    }

    public async Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId)
    {
        return await _context.TimeSlots.FindAsync(timeSlotId);
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByLeagueIdAsync(Guid leagueId)
    {
        return await _context.TimeSlots.Where(timeSlot => timeSlot.LeagueId == leagueId).ToListAsync();
    }

    public async Task AddTimeSlotAsync(TimeSlot timeSlot)
    {
        await _context.TimeSlots.AddAsync(timeSlot);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTimeSlotAsync(TimeSlot timeSlot)
    {
        _context.TimeSlots.Update(timeSlot);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTimeSlotAsync(Guid timeSlotId)
    {
        var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);
        if (timeSlot != null)
        {
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTimeSlotAsyncByLeagueIdAsync(Guid leagueId)
    {
        await _context.TimeSlots
            .Where(timeSlot => timeSlot.LeagueId == leagueId)
            .ExecuteDeleteAsync();
    }
}