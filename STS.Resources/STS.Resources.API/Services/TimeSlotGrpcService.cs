using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.API.Services;

public class TimeSlotGrpcService : TimeSlotService.TimeSlotServiceBase
{
    private readonly ITimeSlotService timeSlotService;

    public TimeSlotGrpcService(ITimeSlotService timeSlotService)
    {
        this.timeSlotService = timeSlotService;
    }

    public override async Task<GetTimeSlotsResponse> GetTimeSlots(GetTimeSlotsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        var timeSlots = await timeSlotService.GetTimeSlotsByLeagueIdAsync(leagueId);

        if (timeSlots == null || !timeSlots.Any())
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No time slots were found for the requested league."));
        }

        var response = new GetTimeSlotsResponse();
        response.TimeSlots.AddRange(timeSlots.Select(MapTimeSlot));
        return response;
    }

    public override async Task<TimeSlotResponse> GetTimeSlot(GetTimeSlotRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        var timeSlot = await timeSlotService.GetTimeSlotByIdAsync(id);

        if (timeSlot == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Time slot was not found."));
        }

        return MapTimeSlot(timeSlot);
    }

    public override async Task<TimeSlotResponse> CreateTimeSlot(CreateTimeSlotRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        var timeSlot = new TimeSlot
        {
            Id = Guid.NewGuid(),
            LeagueId = leagueId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
        };

        try
        {
            await timeSlotService.AddTimeSlotAsync(timeSlot);
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));
        }

        return MapTimeSlot(timeSlot);
    }

    public override async Task<TimeSlotResponse> UpdateTimeSlot(UpdateTimeSlotRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        var timeSlot = await timeSlotService.GetTimeSlotByIdAsync(id);

        if (timeSlot == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Time slot was not found."));
        }

        timeSlot.StartTime = request.StartTime;
        timeSlot.EndTime = request.EndTime;

        try
        {
            await timeSlotService.UpdateTimeSlotAsync(timeSlot);
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }

        return MapTimeSlot(timeSlot);
    }

    public override async Task<Empty> DeleteTimeSlot(DeleteTimeSlotRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        await timeSlotService.DeleteTimeSlotAsync(id);
        return new Empty();
    }

    private static TimeSlotResponse MapTimeSlot(TimeSlot timeSlot)
    {
        return new TimeSlotResponse
        {
            Id = timeSlot.Id.ToString(),
            LeagueId = timeSlot.LeagueId.ToString(),
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime,
        };
    }
}