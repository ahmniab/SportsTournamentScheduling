using System.Text.Json;
using StackExchange.Redis;
using STS.Resources.Application.Models.Responses;
using STS.Resources.Domain.Entities;

namespace STS.Resources.Application.Extensions;

public static class CacheExtensions
{
    public static async Task<string> SetLeagueAsync(this IDatabase db, LeagueResponse league, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var redisKey = $"league:prepared:{league.Id}";
        var serialized = JsonSerializer.Serialize(league);
        await db.StringSetAsync(redisKey, serialized);
        return redisKey;
    }

    public static async Task<LeagueJob?> GetLeagueJobAsync(this IDatabase db, Guid leagueId)
    {
        var leagueJobId = $"jobs:generate_league:{leagueId}";
        var leagueJobString = await db.StringGetAsync(leagueJobId);
        if (leagueJobString.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<LeagueJob>((string)leagueJobString!);
    }

    public static async Task SetLeagueJobAsync(this IDatabase db, LeagueJob leagueJob, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var leagueJobId = $"jobs:generate_league:{leagueJob.LeagueId}";
        leagueJob.UpdatedAt = DateTimeOffset.UtcNow;
        var serializedJob = JsonSerializer.Serialize(leagueJob);
        await db.StringSetAsync(leagueJobId, serializedJob, expiry: TimeSpan.FromMinutes(30));
    }
}
