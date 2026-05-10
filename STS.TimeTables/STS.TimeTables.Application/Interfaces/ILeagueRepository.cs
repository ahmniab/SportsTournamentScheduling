using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Interfaces;

public interface ILeagueRepository
{
    public Task<League?> GetLeagueSummaryAsync(Guid leagueId);
    public Task<League?> GetLeagueFullAsync(Guid leagueId);
    public Task DeleteLeagueAsync(Guid leagueId);
    public Task CreateLeagueAsync(League league);
}