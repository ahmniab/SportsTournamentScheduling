using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Features.TimeSlot;
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
        try
        {
            var timeSlots = await timeSlotService.GetTimeSlotsByLeagueIdAsync(request.LeagueId);
            var response = new GetTimeSlotsResponse();
            response.TimeSlots.AddRange(timeSlots.Select(MapTimeSlot));
            return response;
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
    }

    public override async Task<TimeSlotResponse> GetTimeSlot(GetTimeSlotRequest request, ServerCallContext context)
    {
        try
        {
            var timeSlot = await timeSlotService.GetTimeSlotByIdAsync(request.Id);
            return MapTimeSlot(timeSlot);
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
    }

    public override async Task<TimeSlotResponse> CreateTimeSlot(CreateTimeSlotRequest request, ServerCallContext context)
    {
        try
        {
            var createTimeSlotCommand = new CreateTimeSlotCommand
            {
                LeagueId = request.LeagueId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            var timeSlot = await timeSlotService.CreateTimeSlotAsync(createTimeSlotCommand);
            return MapTimeSlot(timeSlot);
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));
        }
    }

    public override async Task<TimeSlotResponse> UpdateTimeSlot(UpdateTimeSlotRequest request, ServerCallContext context)
    {
        try
        {
            var updateTimeSlotCommand = new UpdateTimeSlotCommand
            {
                Id = request.Id,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            var timeSlot = await timeSlotService.UpdateTimeSlotAsync(updateTimeSlotCommand);
            return MapTimeSlot(timeSlot);
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (KeyNotFoundException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
    }

    public override async Task<Empty> DeleteTimeSlot(DeleteTimeSlotRequest request, ServerCallContext context)
    {
        try
        {
            await timeSlotService.DeleteTimeSlotAsync(request.Id);
            return new Empty();
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
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