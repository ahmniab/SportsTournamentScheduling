using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Attributes;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Features.Stadium;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.API.Services;

public class StadiumGrpcService : StadiumService.StadiumServiceBase
{
    private readonly IStadiumService stadiumService;

    public StadiumGrpcService(IStadiumService stadiumService)
    {
        this.stadiumService = stadiumService;
    }

    public override async Task<GetStadiumsResponse> GetStadiums(GetStadiumsRequest request, ServerCallContext context)
    {
        try
        {
            var stadiums = await stadiumService.GetStadiumsByLeagueIdAsync(request.LeagueId);
            var response = new GetStadiumsResponse();
            response.Stadiums.AddRange(stadiums.Select(MapStadium));
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

    [RequireOwnership(ResourceType.Stadium)]
    public override async Task<StadiumResponse> GetStadium(GetStadiumRequest request, ServerCallContext context)
    {
        try
        {
            var stadium = await stadiumService.GetStadiumByIdAsync(request.Id);
            return MapStadium(stadium);
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
    
    public override async Task<StadiumResponse> CreateStadium(CreateStadiumRequest request, ServerCallContext context)
    {
        try
        {
            var createStadiumCommand = new CreateStadiumCommand
            {
                LeagueId = request.LeagueId,
                Name = request.Name,
                Logo = request.Logo
            };

            var stadium = await stadiumService.CreateStadiumAsync(createStadiumCommand);
            return MapStadium(stadium);
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    [RequireOwnership(ResourceType.Stadium)]
    public override async Task<StadiumResponse> UpdateStadium(UpdateStadiumRequest request, ServerCallContext context)
    {
        try
        {
            var updateStadiumCommand = new UpdateStadiumCommand
            {
                Id = request.Id,
                Name = request.Name,
                Logo = request.Logo
            };

            var stadium = await stadiumService.UpdateStadiumAsync(updateStadiumCommand);
            return MapStadium(stadium);
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

    [RequireOwnership(ResourceType.Stadium)]
    public override async Task<Empty> DeleteStadium(DeleteStadiumRequest request, ServerCallContext context)
    {
        try
        {
            await stadiumService.DeleteStadiumAsync(request.Id);
            return new Empty();
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    private static StadiumResponse MapStadium(Stadium stadium)
    {
        var response = new StadiumResponse
        {
            Id = stadium.Id.ToString(),
            LeagueId = stadium.LeagueId.ToString(),
            Name = stadium.Name,
        };

        if (!string.IsNullOrWhiteSpace(stadium.Logo))
        {
            response.Logo = stadium.Logo;
        }

        return response;
    }
}