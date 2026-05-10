using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STS.BFF.API.Dtos;
using STS.BFF.API.Dtos.Responses;
using STS.BFF.API.Grpc;

namespace STS.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaguesController : ControllerBase
{
    private readonly LeagueService.LeagueServiceClient _leagueService;

    public LeaguesController(LeagueService.LeagueServiceClient leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };
            
            var request = new GetLeaguesRequest { OwnerId = userId };
            var response = await _leagueService.GetLeaguesAsync(request, headers);
            return Ok(response.Leagues.Select(LeagueSummaryDto.From).ToList());
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new GetLeagueRequest { Id = id.ToString() };
            var response = await _leagueService.GetLeagueAsync(request, headers);
            return Ok(LeagueDto.From(response));
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateLeagueDto dto)
    {
        try
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized("No active session found.");
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID (sub) not found in session claims.");
            }
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };
            
            var request = new CreateLeagueRequest
            {
                OwnerId = userId,
                Name = dto.Name,
                StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc)),
            };
            if (dto.LogoUrl != null) request.LogoUrl = dto.LogoUrl;

            var response = await _leagueService.CreateLeagueAsync(request, headers);
            var result = LeagueDto.From(response);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateLeagueDto dto)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };
            
            var request = new UpdateLeagueRequest
            {
                Id = id.ToString(),
                Name = dto.Name,
                StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc)),
            };
            if (dto.LogoUrl != null) request.LogoUrl = dto.LogoUrl;

            var response = await _leagueService.UpdateLeagueAsync(request, headers);
            return Ok(LeagueDto.From(response));
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };
            
            var request = new DeleteLeagueRequest { Id = id.ToString() };
            await _leagueService.DeleteLeagueAsync(request, headers);
            return NoContent();
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
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
