using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.League;
using STS.Resources.Application.Models.Responses;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueService
{
    Task<IEnumerable<League>> GetLeaguesByOwnerIdAsync(String ownerId);
    Task<LeagueResponse> GetLeagueByIdAsync(GetLeagueByIdCommand command);
    Task<bool> VerifyOwnershipAsync(string leagueId, String ownerId);
    Task<League> CreateLeagueAsync(CreateLeagueCommand league);
    Task<League> UpdateLeagueAsync(UpdateLeagueCommand league);
    Task DeleteLeagueAsync(String Id);
}