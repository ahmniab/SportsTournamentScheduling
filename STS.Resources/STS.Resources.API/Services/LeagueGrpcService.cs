using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.API.Services;

public class LeagueGrpcService : LeagueService.LeagueServiceBase
{
    private readonly ILeagueService leagueService;

    public LeagueGrpcService(ILeagueService leagueService)
    {
        this.leagueService = leagueService;
    }

    public override async Task<GetLeaguesResponse> GetLeagues(GetLeaguesRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.OwnerId, out var ownerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "owner_id must be a valid GUID."));
        }

        var leagues = await leagueService.GetLeaguesByOwnerIdAsync(ownerId);

        if (leagues == null || leagues.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No leagues were found for the requested owner."));
        }

        var response = new GetLeaguesResponse();
        response.Leagues.AddRange(leagues.Select(MapLeague));
        return response;
    }

    public override async Task<LeagueResponse> GetLeague(GetLeagueRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        var league = await leagueService.GetLeagueByIdAsync(id);

        if (league == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "League was not found."));
        }

        return MapLeague(league);
    }

    public override async Task<LeagueResponse> CreateLeague(CreateLeagueRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.OwnerId, out var ownerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "owner_id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var league = new League
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = request.Name,
            StartDate = request.StartDate?.ToDateTime() ?? DateTime.UtcNow,
            LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl,
            CreatedAt = DateTime.UtcNow
        };

        await leagueService.CreateLeagueAsync(league);

        return MapLeague(league);
    }

    public override async Task<LeagueResponse> UpdateLeague(UpdateLeagueRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var league = await leagueService.GetLeagueByIdAsync(id);

        if (league == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "League was not found."));
        }

        league.Name = request.Name;
        league.StartDate = request.StartDate?.ToDateTime() ?? league.StartDate;
        league.LogoUrl = string.IsNullOrEmpty(request.LogoUrl) ? null : request.LogoUrl;

        await leagueService.UpdateLeagueAsync(league);

        return MapLeague(league);
    }
    public override async Task<Empty> DeleteLeague(DeleteLeagueRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        await leagueService.DeleteLeagueAsync(id);
        return new Empty();
    }

    private static LeagueResponse MapLeague(League league)
    {
        return new LeagueResponse
        {
            Id = league.Id.ToString(),
            OwnerId = league.OwnerId.ToString(),
            Name = league.Name,
            CreatedAt = league.CreatedAt.ToTimestamp(),
            StartDate = league.StartDate.ToTimestamp(),
            LogoUrl = league.LogoUrl ?? string.Empty
        };
    }
}