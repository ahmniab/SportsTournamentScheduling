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
public class StadiumsController : ControllerBase
{
    private readonly StadiumService.StadiumServiceClient _stadiumService;

    public StadiumsController(StadiumService.StadiumServiceClient stadiumService)
    {
        _stadiumService = stadiumService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid leagueId)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new GetStadiumsRequest { LeagueId = leagueId.ToString() };
            var response = await _stadiumService.GetStadiumsAsync(request, headers);
            return Ok(response.Stadiums.Select(StadiumDto.From).ToList());
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
            var request = new GetStadiumRequest { Id = id.ToString() };
            var response = await _stadiumService.GetStadiumAsync(request, headers);
            return Ok(StadiumDto.From(response));
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
    public async Task<IActionResult> Post([FromBody] CreateStadiumDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new CreateStadiumRequest
            {
                LeagueId = dto.LeagueId.ToString(),
                Name = dto.Name,
            };
            if (dto.Logo != null) request.Logo = dto.Logo;

            var response = await _stadiumService.CreateStadiumAsync(request, headers);
            var result = StadiumDto.From(response);
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
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateStadiumDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new UpdateStadiumRequest
            {
                Id = id.ToString(),
                Name = dto.Name,
            };
            if (dto.Logo != null) request.Logo = dto.Logo;

            var response = await _stadiumService.UpdateStadiumAsync(request, headers);
            return Ok(StadiumDto.From(response));
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
            var request = new DeleteStadiumRequest { Id = id.ToString() };
            await _stadiumService.DeleteStadiumAsync(request, headers);
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
