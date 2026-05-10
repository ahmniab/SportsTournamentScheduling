using System.Text.Json;
using StackExchange.Redis;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Application.Extensions.LeagueJobExtensions;

public static class CacheExtensions
{
    public static async Task SetLeagueJobAsync(this IDatabase db, LeagueJob leagueJob, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var leagueJobId = $"jobs:generate_league:{leagueJob.LeagueId}";
        leagueJob.UpdatedAt = DateTimeOffset.UtcNow;
        var serializedJob = JsonSerializer.Serialize(leagueJob);
        await db.StringSetAsync(leagueJobId, serializedJob, expiry: TimeSpan.FromMinutes(30));
    }

    public static async Task<LeagueJob?> GetLeagueJobAsync(this IDatabase db, Guid leagueId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var leagueJobId = $"jobs:generate_league:{leagueId}";
        var leagueJob = await db.StringGetAsync(leagueJobId);
        if (leagueJob.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<LeagueJob>((string)leagueJob!);
    }
    
}
