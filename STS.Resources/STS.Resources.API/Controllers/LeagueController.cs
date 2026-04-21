using Microsoft.AspNetCore.Mvc;
using STS.Resources.Domain.Entities;
using STS.Resources.Application.Interfaces;
using STS.Resources.Application.DTOs;

namespace STS.Resources.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LeagueController : ControllerBase
{
    private readonly ILeagueService leagueService;

    public LeagueController(ILeagueService leagueService)
    {
        this.leagueService = leagueService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<League>>> GetLeagues(GetLeaguesDto dto)
    {
        var leagues = await this.leagueService.GetLeaguesByOwnerIdAsync(dto.OwnerId);
        return leagues == null || !leagues.Any() ? this.NotFound() : leagues;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<League>> GetLeague(Guid id)
    {
        var league = await this.leagueService.GetLeagueByIdAsync(id);

        if (league == null)
        {
            return this.NotFound();
        }

        return league;
    }

    [HttpPost]
    public async Task<ActionResult> CreateLeague(CreateLeagueDto dto)
    {
        var league = new League
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            OwnerId = dto.OwnerId
        };

        await this.leagueService.CreateLeagueAsync(league);
        return this.CreatedAtAction(nameof(GetLeague), new { id = league.Id }, league);
    }
}
