using Microsoft.EntityFrameworkCore;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.Infrastructure.Repositories;

public class StadiumRepository : IStadiumRepository
{
    private readonly ResourcesDbContext _context;

    public StadiumRepository(ResourcesDbContext context)
    {
        _context = context;
    }
    
    

    public async Task<Stadium?> GetByIdAsync(Guid id)
    {
        return await _context.Stadiums.FindAsync(id);
    }

    public async Task<IEnumerable<Stadium>?> GetByLeagueIdAsync(Guid leagueId)
    {
        return await _context.Stadiums
            .Where(stadium => stadium.LeagueId == leagueId)
            .ToListAsync();
    }

    public async Task<int> GetCountByIdAndOwnerIdWithNoTracksAsync(Guid id, Guid ownerId)
    {
        return await _context.Stadiums.AsNoTracking()
            .Where(stadium => stadium.Id == id)
            .Include(stadium => stadium.League)
            .Where(stadium => stadium.League.OwnerId == ownerId)
            .CountAsync();
    }

    public async Task<Stadium> AddAsync(Stadium stadium)
    {
        await _context.Stadiums.AddAsync(stadium);
        await _context.SaveChangesAsync();
        return stadium;
    }

    public async Task<Stadium> UpdateAsync(Stadium stadium)
    {
        var prevStadium = await _context.Stadiums
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == stadium.Id)
                          ?? throw new KeyNotFoundException("Stadium was not found after update.");

        stadium.LeagueId = prevStadium.LeagueId;
        _context.Stadiums.Update(stadium);
        await _context.SaveChangesAsync();
        return stadium;
    }

    public async Task DeleteAsync(Guid id)
    {
        var stadium = await GetByIdAsync(id);
        if (stadium != null)
        {
            _context.Stadiums.Remove(stadium);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteStadiumAsyncByLeagueIdAsync(Guid leagueId)
    {
        await _context.Stadiums
            .Where(stadium => stadium.LeagueId == leagueId)
            .ExecuteDeleteAsync();
    }
}