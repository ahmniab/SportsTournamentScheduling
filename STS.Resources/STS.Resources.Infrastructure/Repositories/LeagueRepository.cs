using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Persistence;
using STS.Resources.Application.Features.League;
using Microsoft.EntityFrameworkCore;
using STS.Resources.Application.Features;

namespace STS.Resources.Infrastructure.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly ResourcesDbContext _context;

    public LeagueRepository(ResourcesDbContext context)
    {
        _context = context;
    }

    public async Task<League?> GetByIdAsync(Guid id)
    {
        return await _context.Leagues.FindAsync(id);
    }
    public async Task<League?> GetByIdAsync(
        Guid id, 
        bool includeTeams = false,  
        bool includeStadiums = false,
        bool includeTimeSlots = false)
    {
        var league =  _context.Leagues.AsQueryable();

        if (includeTeams)
            league = league.Include(l => l.Teams);
        if (includeStadiums)
            league = league.Include(l => l.Stadiums);
        if (includeTimeSlots)
            league = league.Include(l => l.TimeSlots);
        
        return await league.FirstOrDefaultAsync(l => l.Id == id);
        
    }

    public async Task<IEnumerable<League>?> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Leagues
            .Where(l => l.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<League> AddAsync(League league)
    {
        await _context.Leagues.AddAsync(league);
        await _context.SaveChangesAsync();
        return league;
    }

    public async Task<League> UpdateAsync(League league)
    {
        var prevLeague = await _context.Leagues
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == league.Id) 
                         ?? throw new KeyNotFoundException("League was not found after update.");
        league.OwnerId = prevLeague.OwnerId;
        league.CreatedAt = prevLeague.CreatedAt;
        _context.Leagues.Update(league);

        await _context.SaveChangesAsync();
        return league;
    }

    public async Task DeleteAsync(Guid Id)
    {
        var league = await GetByIdAsync(Id);
        if (league != null)
        {
            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();
        }
    }
}