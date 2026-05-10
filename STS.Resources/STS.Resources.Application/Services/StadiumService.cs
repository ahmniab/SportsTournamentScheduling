using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.Stadium;
using STS.Resources.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using STS.Resources.Application.Features;

namespace STS.Resources.Application.Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ILeaguePermissionService _permissionService;

    public StadiumService(IStadiumRepository stadiumRepository,  ILeaguePermissionService permissionService)
    {
        _stadiumRepository = stadiumRepository;
        _permissionService = permissionService;
    }

    public async Task<Stadium> GetStadiumByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var stadiumGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }

        return await _stadiumRepository.GetByIdAsync(stadiumGuid) ?? throw new KeyNotFoundException("Stadium was not found.");
    }

    public async Task<IEnumerable<Stadium>> GetStadiumsByLeagueIdAsync(string leagueId)
    {
        if (!Guid.TryParse(leagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(leagueId));
        }

        var stadiums = await _stadiumRepository.GetByLeagueIdAsync(leagueGuid);
        return stadiums ?? throw new KeyNotFoundException("No stadiums were found for the requested league.");
    }

    public async Task<Stadium> CreateStadiumAsync(CreateStadiumCommand stadium)
    {
        if (!Guid.TryParse(stadium.LeagueId, out var leagueGuid))
        {
            throw new ArgumentException("league_id must be a valid GUID.", nameof(stadium.LeagueId));
        }

        if (string.IsNullOrWhiteSpace(stadium.Name))
        {
            throw new ArgumentException("name is required.", nameof(stadium.Name));
        }

        var newStadium = new Stadium
        {
            LeagueId = leagueGuid,
            Name = stadium.Name,
            Logo = stadium.Logo
        };

        await _stadiumRepository.AddAsync(newStadium);
        // await _permissionService.AddResourceAsync(newStadium.LeagueId.ToString(), newStadium.Id.ToString());
        return newStadium;
    }

    public async Task<Stadium> UpdateStadiumAsync(UpdateStadiumCommand stadium)
    {
        if (!Guid.TryParse(stadium.Id, out var stadiumGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(stadium.Id));
        }

        if (string.IsNullOrWhiteSpace(stadium.Name))
        {
            throw new ArgumentException("name is required.", nameof(stadium.Name));
        }

        try
        {
            var updatedStadium = new Stadium
            {
                Id = stadiumGuid,
                Name = stadium.Name,
                Logo = stadium.Logo
            };

            await _stadiumRepository.UpdateAsync(updatedStadium);
            return updatedStadium;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new KeyNotFoundException("Stadium was not found or has been deleted.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Stadium was not found.", ex);
        }
    }

    public async Task DeleteStadiumAsync(string id)
    {
        if (!Guid.TryParse(id, out var stadiumGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }
        // var stadium = await _stadiumRepository.GetByIdAsync(stadiumGuid);
        await _stadiumRepository.DeleteAsync(stadiumGuid);
        // if (stadium != null)
        //     await _permissionService.DeleteResourceAsync(stadium.LeagueId.ToString(), stadium.Id.ToString());
    }

    public async Task<bool> VerifyOwnershipAsync(string stadiumId, string ownerId)
    {
        if (!Guid.TryParse(stadiumId, out var stadiumGuid))
        {
            throw new ArgumentException("id must be a guid");
        }

        if (!Guid.TryParse(ownerId, out var ownerGuid))
        {
            throw new ArgumentException("owner_id must be a guid");
        }

        var stadiumCount
            = await _stadiumRepository.GetCountByIdAndOwnerIdWithNoTracksAsync(stadiumGuid, ownerGuid);
        return stadiumCount > 0;
        
    }
}