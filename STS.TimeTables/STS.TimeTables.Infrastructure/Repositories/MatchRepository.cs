using STS.TimeTables.Application.Interfaces;
using STS.TimeTables.Domain.Entities;
using STS.TimeTables.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace STS.TimeTables.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TimeTableDbContext _dbContext;
    public MatchRepository(TimeTableDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Match>> GetMatchesAsync(Guid leagueId)
    {
        return await _dbContext.Matches
            .Where(m => m.LeagueId == leagueId)
            .ToListAsync<Match>();

    }

    public async Task<Match?> GetMatchAsync(Guid id)
    {
        return await _dbContext.Matches
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}