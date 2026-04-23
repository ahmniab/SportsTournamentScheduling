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

    public async Task<Team?> GetTeamByIdAsync(Guid Id)
    {
        return await _context.Teams.FindAsync(Id);
    }

    public async Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(Guid leagueId)
    {
        var teams = await _context.Teams.Where(t => t.LeagueId == leagueId).ToListAsync();
        return teams == null || !teams.Any() ? Enumerable.Empty<Team>() : teams;
    }

    public async Task AddTeamAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTeamAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTeamAsync(Guid teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}