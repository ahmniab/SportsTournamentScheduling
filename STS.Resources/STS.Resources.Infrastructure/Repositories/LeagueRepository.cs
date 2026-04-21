using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<League>?> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Leagues.Where(l => l.OwnerId == ownerId).ToListAsync();
    }

    public async Task AddAsync(League league)
    {
        await _context.Leagues.AddAsync(league);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(League league)
    {
        _context.Leagues.Update(league);
        await _context.SaveChangesAsync();
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