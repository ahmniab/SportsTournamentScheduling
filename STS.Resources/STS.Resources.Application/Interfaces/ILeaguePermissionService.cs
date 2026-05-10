using STS.Resources.API.Attributes;

namespace STS.Resources.Application.Interfaces;

public interface ILeaguePermissionService
{
    public Task<bool> HasAccessAsync(string resourceId, string userId, AccessLevel accessLevel);
    public Task AddAccessAsync(string league, string userId, AccessLevel accessLevel);
    public Task AddResourceAsync(string leagueId, string resourceId);
    public Task RemoveAccessAsync(string league, string userId);
    public Task DeleteLeagueAsync(string leagueId);
    public Task DeleteResourceAsync(string leagueId, string resourceId);
}