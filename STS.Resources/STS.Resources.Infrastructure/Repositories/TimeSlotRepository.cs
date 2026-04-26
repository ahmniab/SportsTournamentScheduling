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

    public async Task<TimeSlot?> GetByIdAsync(Guid id)
    {
        return await _context.TimeSlots.FindAsync(id);
    }
    
    public async Task<int> GetCountByIdAndOwnerIdWithNoTracksAsync(Guid id, Guid ownerId)
    {
        return await _context.TimeSlots.AsNoTracking()
            .Where(ts => ts.Id == id)
            .Include(ls => ls.League)
            .Where(ts => ts.League.OwnerId == ownerId)
            .CountAsync();
    }

    public async Task<IEnumerable<TimeSlot>?> GetByLeagueIdAsync(Guid leagueId)
    {
        return await _context.TimeSlots.Where(timeSlot => timeSlot.LeagueId == leagueId).ToListAsync();
    }

    public async Task<TimeSlot> AddAsync(TimeSlot timeSlot)
    {
        await _context.TimeSlots.AddAsync(timeSlot);
        await _context.SaveChangesAsync();
        return timeSlot;
    }

    public async Task<TimeSlot> UpdateAsync(TimeSlot timeSlot)
    {
        var prevTimeSlot = await _context.TimeSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == timeSlot.Id)
                           ?? throw new KeyNotFoundException("Time slot was not found after update.");

        timeSlot.LeagueId = prevTimeSlot.LeagueId;
        _context.TimeSlots.Update(timeSlot);
        await _context.SaveChangesAsync();
        return timeSlot;
    }

    public async Task DeleteAsync(Guid id)
    {
        var timeSlot = await GetByIdAsync(id);
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