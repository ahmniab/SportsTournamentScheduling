using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
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
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        var stadiums = await stadiumService.GetStadiumsByLeagueIdAsync(leagueId);

        if (stadiums == null || !stadiums.Any())
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No stadiums were found for the requested league."));
        }

        var response = new GetStadiumsResponse();
        response.Stadiums.AddRange(stadiums.Select(MapStadium));
        return response;
    }

    public override async Task<StadiumResponse> GetStadium(GetStadiumRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        var stadium = await stadiumService.GetStadiumByIdAsync(id);

        if (stadium == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Stadium was not found."));
        }

        return MapStadium(stadium);
    }

    public override async Task<StadiumResponse> CreateStadium(CreateStadiumRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var stadium = new Stadium
        {
            Id = Guid.NewGuid(),
            LeagueId = leagueId,
            Name = request.Name,
            Logo = request.Logo,
        };

        await stadiumService.AddStadiumAsync(stadium);

        return MapStadium(stadium);
    }

    public override async Task<StadiumResponse> UpdateStadium(UpdateStadiumRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var stadium = await stadiumService.GetStadiumByIdAsync(id);

        if (stadium == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Stadium was not found."));
        }

        stadium.Name = request.Name;
        stadium.Logo = request.Logo;

        await stadiumService.UpdateStadiumAsync(stadium);

        return MapStadium(stadium);
    }

    public override async Task<Empty> DeleteStadium(DeleteStadiumRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        await stadiumService.DeleteStadiumAsync(id);
        return new Empty();
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