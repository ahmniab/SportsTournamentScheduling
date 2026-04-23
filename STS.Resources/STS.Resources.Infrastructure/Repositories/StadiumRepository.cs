using System.Runtime.InteropServices;
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

    public async Task<Stadium?> GetStadiumByIdAsync(Guid stadiumId)
    {
        return await _context.Stadiums.FindAsync(stadiumId);
    }

    public async Task<IEnumerable<Stadium>> GetStadiumsByLeagueIdAsync(Guid leagueId)
    {
        var stadiums = await _context.Stadiums.Where(stadium => stadium.LeagueId == leagueId).ToListAsync();
        return stadiums == null || !stadiums.Any() ? Enumerable.Empty<Stadium>() : stadiums;
    }

    public async Task AddStadiumAsync(Stadium stadium)
    {
        await _context.Stadiums.AddAsync(stadium);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStadiumAsync(Stadium stadium)
    {
        _context.Stadiums.Update(stadium);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStadiumAsync(Guid stadiumId)
    {
        var stadium = await _context.Stadiums.FindAsync(stadiumId);
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