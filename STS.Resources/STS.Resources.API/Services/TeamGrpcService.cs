using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.Resources.API.Grpc;
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
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        var teams = await teamService.GetTeamsByLeagueIdAsync(leagueId);

        if (teams == null || !teams.Any())
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No teams were found for the requested league."));
        }

        var response = new GetTeamsResponse();
        response.Teams.AddRange(teams.Select(MapTeam));
        return response;
    }

    public override async Task<TeamResponse> GetTeam(GetTeamRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        var team = await teamService.GetTeamByIdAsync(id);

        if (team == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Team was not found."));
        }

        return MapTeam(team);
    }

    public override async Task<TeamResponse> CreateTeam(CreateTeamRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.LeagueId, out var leagueId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "league_id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var team = new Team
        {
            Id = Guid.NewGuid(),
            LeagueId = leagueId,
            Name = request.Name,
            LogoUrl = request.LogoUrl,
            CreatedAt = DateTime.UtcNow
        };

        await teamService.AddTeamAsync(team);

        return MapTeam(team);
    }

    public override async Task<TeamResponse> UpdateTeam(UpdateTeamRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "name is required."));
        }

        var team = await teamService.GetTeamByIdAsync(id);

        if (team == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Team was not found."));
        }

        team.Name = request.Name;
        team.LogoUrl = request.LogoUrl;

        await teamService.UpdateTeamAsync(team);

        return MapTeam(team);
    }

    public override async Task<Empty> DeleteTeam(DeleteTeamRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be a valid GUID."));
        }

        await teamService.DeleteTeamAsync(id);
        return new Empty();
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
