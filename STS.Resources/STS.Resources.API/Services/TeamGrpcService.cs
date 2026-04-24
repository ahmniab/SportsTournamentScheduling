using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Features.Team;
using STS.Resources.Application.Interfaces;
using STS.Resources.Domain.Entities;

namespace STS.Resources.API.Services;

public class TeamGrpcService : TeamService.TeamServiceBase
{
    private readonly ITeamService teamService;

    public TeamGrpcService(ITeamService teamService)
    {
        this.teamService = teamService;
    }

    public override async Task<GetTeamsResponse> GetTeams(GetTeamsRequest request, ServerCallContext context)
    {
        try
        {
            var teams = await teamService.GetTeamsByLeagueIdAsync(request.LeagueId);
            var response = new GetTeamsResponse();
            response.Teams.AddRange(teams.Select(MapTeam));
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

    public override async Task<TeamResponse> GetTeam(GetTeamRequest request, ServerCallContext context)
    {
        try
        {
            var team = await teamService.GetTeamByIdAsync(request.Id);
            return MapTeam(team);
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

    public override async Task<TeamResponse> CreateTeam(CreateTeamRequest request, ServerCallContext context)
    {
        try
        {
            var createTeamCommand = new CreateTeamCommand
            {
                LeagueId = request.LeagueId,
                Name = request.Name,
                LogoUrl = request.LogoUrl
            };

            var team = await teamService.CreateTeamAsync(createTeamCommand);
            return MapTeam(team);
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    public override async Task<TeamResponse> UpdateTeam(UpdateTeamRequest request, ServerCallContext context)
    {
        try
        {
            var updateTeamCommand = new UpdateTeamCommand
            {
                Id = request.Id,
                Name = request.Name,
                LogoUrl = request.LogoUrl
            };

            var team = await teamService.UpdateTeamAsync(updateTeamCommand);
            return MapTeam(team);
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

    public override async Task<Empty> DeleteTeam(DeleteTeamRequest request, ServerCallContext context)
    {
        try
        {
            await teamService.DeleteTeamAsync(request.Id);
            return new Empty();
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    private static TeamResponse MapTeam(Team team)
    {
        var response = new TeamResponse
        {
            Id = team.Id.ToString(),
            LeagueId = team.LeagueId.ToString(),
            Name = team.Name,
        };
        if (!string.IsNullOrWhiteSpace(team.LogoUrl))
            response.LogoUrl = team.LogoUrl;
        return response;
    }
}
