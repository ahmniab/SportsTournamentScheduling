using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.Features.League;
using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Application.Services;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public LeagueService(ILeagueRepository leagueRepository, ITeamRepository teamRepository, IStadiumRepository stadiumRepository, ITimeSlotRepository timeSlotRepository)
    {
        _leagueRepository = leagueRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<League> GetLeagueByIdAsync(String id)
    {
        if (!Guid.TryParse(id, out var leagueGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(id));
        }
        return await _leagueRepository.GetByIdAsync(leagueGuid) ?? throw new KeyNotFoundException("League was not found.");
    }

    public async Task<IEnumerable<League>> GetLeaguesByOwnerIdAsync(string ownerId)
    {
        if (!Guid.TryParse(ownerId, out var ownerGuid))
        {
            throw new ArgumentException("owner_id must be a valid GUID.", nameof(ownerId));
        }

        var leagues = await _leagueRepository.GetByOwnerIdAsync(ownerGuid);
        return leagues ?? throw new KeyNotFoundException("No leagues were found for the requested owner.");
    }
    public async Task<League> CreateLeagueAsync(CreateLeagueCommand league)
    {
        if (!Guid.TryParse(league.OwnerId, out var ownerGuid))
        {
            throw new ArgumentException("owner_id must be a valid GUID.", nameof(league.OwnerId));
        }
        if (string.IsNullOrWhiteSpace(league.Name))
        {
            throw new ArgumentException("name is required.", nameof(league.Name));
        }
        if (league.StartDate == DateTime.MinValue)
        {
            throw new ArgumentException("start_date is required.", nameof(league.StartDate));
        }
        var newLeague = new League
        {
            OwnerId = ownerGuid,
            Name = league.Name,
            StartDate = league.StartDate,
            CreatedAt = DateTime.UtcNow,
            LogoUrl = league.LogoUrl
        };
        return await _leagueRepository.AddAsync(newLeague);
    }

    public async Task<League> UpdateLeagueAsync(UpdateLeagueCommand league)
    {
        if (!Guid.TryParse(league.Id, out var leagueGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(league.Id));
        }
        if (string.IsNullOrWhiteSpace(league.Name))
        {
            throw new ArgumentException("name is required.", nameof(league.Name));
        }
        if (league.StartDate == DateTime.MinValue)
        {
            throw new ArgumentException("start_date is required.", nameof(league.StartDate));
        }
        try
        {
            var updatedLeague = new League
            {
                Id = leagueGuid,
                Name = league.Name,
                StartDate = league.StartDate,
                LogoUrl = league.LogoUrl
            };
            await _leagueRepository.UpdateAsync(updatedLeague);
            return updatedLeague;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new KeyNotFoundException("League was not found or has been deleted.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("League was not found.", ex);
        }
    }

    public async Task DeleteLeagueAsync(String Id)
    {
        if (!Guid.TryParse(Id, out var leagueGuid))
        {
            throw new ArgumentException("id must be a valid GUID.", nameof(Id));
        }

        await _teamRepository.DeleteTeamAsyncByLeagueIdAsync(leagueGuid);
        await _stadiumRepository.DeleteStadiumAsyncByLeagueIdAsync(leagueGuid);
        await _timeSlotRepository.DeleteTimeSlotAsyncByLeagueIdAsync(leagueGuid);
        await _leagueRepository.DeleteAsync(leagueGuid);
    }
}