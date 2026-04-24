using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;
using STS.Resources.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ResourcesDbContext _context;

    public TeamRepository(ResourcesDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams.FindAsync(id);
    }

    public async Task<IEnumerable<Team>?> GetByLeagueIdAsync(Guid leagueId)
    {
        return await _context.Teams
            .Where(t => t.LeagueId == leagueId)
            .ToListAsync();
    }

    public async Task<Team> AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Team team)
    {
        var prevTeam = await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == team.Id)
                       ?? throw new KeyNotFoundException("Team was not found after update.");

        team.LeagueId = prevTeam.LeagueId;
        team.CreatedAt = prevTeam.CreatedAt;
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task DeleteAsync(Guid id)
    {
        var team = await GetByIdAsync(id);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTeamAsyncByLeagueIdAsync(Guid leagueId)
    {
        await _context.Teams
            .Where(team => team.LeagueId == leagueId)
            .ExecuteDeleteAsync();
    }
}