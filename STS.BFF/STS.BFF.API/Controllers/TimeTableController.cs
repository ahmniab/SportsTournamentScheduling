
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using STS.BFF.API.Features.League.Commands;
using STS.TimeTables.API.Grpc;

namespace STS.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeTableController : ControllerBase
{
    private readonly TimeTablesService.TimeTablesServiceClient _timeTablesService;
    private readonly GetFullTimeTableCommandHandler _getFullTimeTableCommandHandler;

    public TimeTableController(
        TimeTablesService.TimeTablesServiceClient timeTablesService,
        GetFullTimeTableCommandHandler getFullTimeTableCommandHandler)
    {
        _timeTablesService = timeTablesService;
        _getFullTimeTableCommandHandler = getFullTimeTableCommandHandler;
    }

    [HttpGet("league-summary/{leagueId}")]
    public async Task<IActionResult> GetLeagueSummary(Guid leagueId)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new LeagueRequest { Id = leagueId.ToString() };
            var response = await _timeTablesService.GetLeagueSummaryAsync(request, headers);
            return Ok(response);
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

    [HttpGet("full-league/{leagueId}")]
    public async Task<IActionResult> GetFullLeague(Guid leagueId)
    {
        try
        {
            var response = await
                _getFullTimeTableCommandHandler.HandleAsync(
                    new GetFullTimeTableCommand
                    {
                        LeagueId = leagueId
                    });
            return Ok(response);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.Unauthenticated)
        {
            return Unauthorized();
        }
        catch (UnauthorizedAccessException ex)
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

    [HttpDelete("{leagueId}")]
    public async Task<IActionResult> DeleteLeague(Guid leagueId)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new LeagueRequest { Id = leagueId.ToString() };
            await _timeTablesService.DeleteLeagueAsync(request, headers);
            return NoContent();
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

    [HttpGet("match/{matchId}")]
    public async Task<IActionResult> GetMatch(Guid matchId)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new MatchRequest { Id = matchId.ToString() };
            var response = await _timeTablesService.GetMatchRequestAsync(request, headers);
            return Ok(response);
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

    [HttpPost("generate/{leagueId}")]
    public async Task<IActionResult> GenerateTimeTable(Guid leagueId)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new GenerateTimeTableRequest { LeagueId = leagueId.ToString() };
            await _timeTablesService.GenerateTimeTableAsync(request, headers);
            return NoContent();
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

    [HttpGet("job-status/{leagueId}")]
    public async Task<IActionResult> GetLeagueJobStatus(Guid leagueId)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

            var request = new LeagueRequest { Id = leagueId.ToString() };
            var response = await _timeTablesService.GetLeagueJobStatusAsync(request, headers);
            return Ok(response);
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
}