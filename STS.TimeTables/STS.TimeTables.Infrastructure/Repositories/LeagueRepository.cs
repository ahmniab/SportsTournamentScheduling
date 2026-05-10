using Microsoft.EntityFrameworkCore;
using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;
using STS.TimeTables.Infrastructure.Persistence;


namespace STS.TimeTables.Infrastructure.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly TimeTableDbContext _dbContext;
    public LeagueRepository(TimeTableDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<League?> GetLeagueSummaryAsync(Guid Id)
    {
        return await _dbContext.Leagues.FirstOrDefaultAsync(league => league.Id == Id);
    }

    public async Task<League?> GetLeagueFullAsync(Guid leagueId)
    {
        return await _dbContext.Leagues
            .Where(l => l.Id == leagueId)
            .Include(l => l.Matches)
            .FirstOrDefaultAsync();
    }

    public Task DeleteLeagueAsync(Guid leagueId)
        => _dbContext.Leagues.Where(l => l.Id == leagueId).ExecuteDeleteAsync();

    public async Task CreateLeagueAsync(League league)
    {
        await _dbContext.Leagues.AddAsync(league);
        await _dbContext.SaveChangesAsync();
    }
}