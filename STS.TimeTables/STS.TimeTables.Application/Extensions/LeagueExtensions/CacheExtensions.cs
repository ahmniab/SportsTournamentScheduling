using System.Text.Json;
using StackExchange.Redis;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Extensions.LeagueExtensions;

public static class CacheExtensions
{
    public static async Task<League?> GetScheduledLeagueAsync(this IDatabase db, Guid leagueId)
    {
        var redisKey = $"league:scheduled:{leagueId}";
        var leagueJson = await db.StringGetAsync(redisKey);

        if (leagueJson.IsNullOrEmpty) 
        {
            return null;
        }
        
        var league = JsonSerializer.Deserialize<League>((string)leagueJson!);
        
        if (league != null)
        {
            league.GeneratedAt = DateTime.SpecifyKind(league.GeneratedAt, DateTimeKind.Utc);
            
            if (league.Matches != null)
            {
                foreach (var match in league.Matches)
                {
                    match.Date = DateTime.SpecifyKind(match.Date, DateTimeKind.Utc);
                }
            }
        }
        
        return league;
    }
}