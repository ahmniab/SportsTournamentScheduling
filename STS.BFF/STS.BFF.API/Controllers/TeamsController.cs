using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STS.BFF.API.Dtos;
using STS.BFF.API.Dtos.Responses;
using STS.BFF.API.Grpc;

namespace STS.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly TeamService.TeamServiceClient _teamService;

    public TeamsController(TeamService.TeamServiceClient teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid leagueId)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new GetTeamsRequest { LeagueId = leagueId.ToString() };
            var response = await _teamService.GetTeamsAsync(request, headers);
            return Ok(response.Teams.Select(TeamDto.From).ToList());
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new GetTeamRequest { Id = id.ToString() };
            var response = await _teamService.GetTeamAsync(request, headers);
            return Ok(TeamDto.From(response));
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTeamDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new CreateTeamRequest
            {
                LeagueId = dto.LeagueId.ToString(),
                Name = dto.Name,
            };
            if (dto.LogoUrl != null) request.LogoUrl = dto.LogoUrl;

            var response = await _teamService.CreateTeamAsync(request, headers);
            var result = TeamDto.From(response);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTeamDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new UpdateTeamRequest
            {
                Id = id.ToString(),
                Name = dto.Name,
            };
            if (dto.LogoUrl != null) request.LogoUrl = dto.LogoUrl;

            var response = await _teamService.UpdateTeamAsync(request, headers);
            return Ok(TeamDto.From(response));
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new DeleteTeamRequest { Id = id.ToString() };
            await _teamService.DeleteTeamAsync(request, headers);
            return NoContent();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
    }
}
