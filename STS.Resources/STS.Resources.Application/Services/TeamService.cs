using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.Team;
using STS.Resources.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using STS.Resources.API.Attributes;

namespace STS.Resources.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ILeaguePermissionService _permissionService;

    public TeamService(ITeamRepository teamRepository, ILeaguePermissionService permissionService)
    {
        _teamRepository = teamRepository;
        _permissionService = permissionService;
    }

    public async Task<Team> GetTeamByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var teamGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }

        return await _teamRepository.GetByIdAsync(teamGuid) ?? throw new KeyNotFoundException("Team was not found.");
    }

    public async Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(string leagueId)
    {
        if (!Guid.TryParse(leagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(leagueId));
        }

        var teams = await _teamRepository.GetByLeagueIdAsync(leagueGuid);
        return teams ?? throw new KeyNotFoundException("No teams were found for the requested league.");
    }

    public async Task<bool> VerifyOwnershipAsync(string teamId, string ownerId)
    {
        if (!Guid.TryParse(teamId, out var teamGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(teamId));
        }

        if (!Guid.TryParse(ownerId, out var ownerGuid))
        {
            throw new ArgumentException("owner_id must be a valid GUID.", nameof(ownerId));
        }
        var teamCount = await _teamRepository.GetCountByIdAndOwnerIdWithNoTracksAsync(teamGuid, ownerGuid);
        return teamCount > 0;
        
    }

    public async Task<Team> CreateTeamAsync(CreateTeamCommand team)
    {
        if (!Guid.TryParse(team.LeagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(team.LeagueId));
        }

        if (string.IsNullOrWhiteSpace(team.Name))
        {
            throw new ArgumentException("name is required.", nameof(team.Name));
        }

        var newTeam = new Team
        {
            LeagueId = leagueGuid,
            Name = team.Name,
            CreatedAt = DateTime.UtcNow,
            LogoUrl = team.LogoUrl
        };

        await _teamRepository.AddAsync(newTeam);
        // await _permissionService.AddAccessAsync(newTeam.LeagueId.ToString(), newTeam.Id.ToString(), AccessLevel.Owner);
        return newTeam;
    }

    public async Task<Team> UpdateTeamAsync(UpdateTeamCommand team)
    {
        if (!Guid.TryParse(team.Id, out var teamGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(team.Id));
        }

        if (string.IsNullOrWhiteSpace(team.Name))
        {
            throw new ArgumentException("name is required.", nameof(team.Name));
        }

        try
        {
            var updatedTeam = new Team
            {
                Id = teamGuid,
                Name = team.Name,
                LogoUrl = team.LogoUrl
            };

            await _teamRepository.UpdateAsync(updatedTeam);
            return updatedTeam;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new KeyNotFoundException("Team was not found or has been deleted.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Team was not found.", ex);
        }
    }

    public async Task DeleteTeamAsync(string id)
    {
        if (!Guid.TryParse(id, out var teamGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }

        // var team = await _teamRepository.GetByIdAsync(teamGuid);
        await _teamRepository.DeleteAsync(teamGuid);
        // if(team != null) 
        //     await _permissionService.DeleteResourceAsync(team.LeagueId.ToString(), team.Id.ToString());
    }
}