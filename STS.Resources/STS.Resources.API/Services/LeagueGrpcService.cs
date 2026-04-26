using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Attributes;
using STS.Resources.API.Grpc;
using STS.Resources.API.Extentions;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;
using STS.Resources.Application.Features.League;

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
        try
        {
            var leagues = await leagueService.GetLeaguesByOwnerIdAsync(request.OwnerId);
            var response = new GetLeaguesResponse();
            response.Leagues.AddRange(leagues.Select(MapLeagueSummary));
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

    [RequireOwnership(ResourceType.League)]
    public override async Task<LeagueResponse> GetLeague(GetLeagueRequest request, ServerCallContext context)
    {
        try
        {
            var command = request.ToGetLeagueByIdCommand();
            var leagueResponse = await leagueService.GetLeagueByIdAsync(command);
            return leagueResponse.ToGrpcResponse(command.IncludeOptions);
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
    
    public override async Task<LeagueResponse> CreateLeague(CreateLeagueRequest request, ServerCallContext context)
    {
        try
        {
            var createLeagueCommand = new CreateLeagueCommand
            {
                OwnerId = request.OwnerId,
                Name = request.Name,
                StartDate = request.StartDate?.ToDateTime() ?? DateTime.MinValue,
                LogoUrl = request.LogoUrl
            };
            var league = await leagueService.CreateLeagueAsync(createLeagueCommand);
            return MapLeague(league);
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }

    }

    [RequireOwnership(ResourceType.League)]
    public override async Task<LeagueResponse> UpdateLeague(UpdateLeagueRequest request, ServerCallContext context)
    {
        try
        {
            var updateLeagueCommand = new UpdateLeagueCommand
            {
                Id = request.Id,
                Name = request.Name,
                StartDate = request.StartDate?.ToDateTime() ?? DateTime.MinValue,
                LogoUrl = request.LogoUrl
            };
            var league = await leagueService.UpdateLeagueAsync(updateLeagueCommand);
            return MapLeague(league);
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
    public override async Task<Empty> DeleteLeague(DeleteLeagueRequest request, ServerCallContext context)
    {
        try
        {
            await leagueService.DeleteLeagueAsync(request.Id);
            return new Empty();
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    private static LeagueResponse MapLeague(League league)
    {
        var response = new LeagueResponse
        {
            Id = league.Id.ToString(),
            OwnerId = league.OwnerId.ToString(),
            Name = league.Name,
            CreatedAt = ToUtcTimestamp(league.CreatedAt),
            StartDate = ToUtcTimestamp(league.StartDate),
        };
        if (!string.IsNullOrWhiteSpace(league.LogoUrl))
            response.LogoUrl = league.LogoUrl;

        return response;
    }

    private static LeagueResponseSummary MapLeagueSummary(League league)
    {
        return new LeagueResponseSummary
        {
            Id = league.Id.ToString(),
            Name = league.Name,
            CreatedAt = ToUtcTimestamp(league.CreatedAt),
            StartDate = ToUtcTimestamp(league.StartDate),
        };
    }

    private static Timestamp ToUtcTimestamp(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return utcValue.ToTimestamp();
    }
}