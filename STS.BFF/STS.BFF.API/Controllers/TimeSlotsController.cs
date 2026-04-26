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
public class TimeSlotsController : ControllerBase
{
    private readonly TimeSlotService.TimeSlotServiceClient _timeSlotService;

    public TimeSlotsController(TimeSlotService.TimeSlotServiceClient timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid leagueId)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new GetTimeSlotsRequest { LeagueId = leagueId.ToString() };
            var response = await _timeSlotService.GetTimeSlotsAsync(request, headers);
            return Ok(response.TimeSlots.Select(TimeSlotDto.From).ToList());
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
            var request = new GetTimeSlotRequest { Id = id.ToString() };
            var response = await _timeSlotService.GetTimeSlotAsync(request, headers);
            return Ok(TimeSlotDto.From(response));
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
    public async Task<IActionResult> Post([FromBody] CreateTimeSlotDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new CreateTimeSlotRequest
            {
                LeagueId = dto.LeagueId.ToString(),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };

            var response = await _timeSlotService.CreateTimeSlotAsync(request, headers);
            var result = TimeSlotDto.From(response);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.FailedPrecondition)
        {
            return Conflict(ex.Message);
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.PermissionDenied)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTimeSlotDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var headers = new Metadata { { "x-owner-id", userId.Value } };
            var request = new UpdateTimeSlotRequest
            {
                Id = id.ToString(),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };

            var response = await _timeSlotService.UpdateTimeSlotAsync(request, headers);
            return Ok(TimeSlotDto.From(response));
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
            var request = new DeleteTimeSlotRequest { Id = id.ToString() };
            await _timeSlotService.DeleteTimeSlotAsync(request, headers);
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
