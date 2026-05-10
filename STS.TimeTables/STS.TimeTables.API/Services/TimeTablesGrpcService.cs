using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using STS.TimeTables.API.Grpc;
using STS.TimeTables.API.Mappers;
using STS.TimeTables.Application.Features.LeagueCommands;
using STS.TimeTables.Application.Features.MatchCommands;

namespace STS.TimeTables.API.Services;

public class TimeTablesGrpcService : TimeTablesService.TimeTablesServiceBase
{
    private readonly GetLeagueSummaryCommandHandler _getLeagueSummary;
    private readonly GetFullLeagueCommandHandler    _getFullLeague;
    private readonly DeleteLeagueCommandHandler     _deleteLeague;
    private readonly GetMatchCommandHandler         _getMatch;
    private readonly GenerateTimeTableHandler       _generateTimeTable;

    public TimeTablesGrpcService(
        GetLeagueSummaryCommandHandler getLeagueSummary,
        GetFullLeagueCommandHandler    getFullLeague,
        DeleteLeagueCommandHandler     deleteLeague,
        GetMatchCommandHandler         getMatch, 
        GenerateTimeTableHandler  generateTimeTable)
    {
        _getLeagueSummary   = getLeagueSummary;
        _getFullLeague      = getFullLeague;
        _deleteLeague       = deleteLeague;
        _getMatch           = getMatch;
        _generateTimeTable = generateTimeTable;
    }

    public override async Task<GetLeagueSummaryResponse> GetLeagueSummary(
        LeagueRequest request, ServerCallContext context)
    {
        try
        {
            var league = await _getLeagueSummary.Handle(
                new GetLeagueSummaryCommand { LeagueId = request.Id },
                context.CancellationToken);

            return league.ToSummaryResponse();
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

    public override async Task<GetFullLeagueResponse> GetFullLeague(
        LeagueRequest request, ServerCallContext context)
    {
        try
        {
            var league = await _getFullLeague.Handle(
                new GetFullLeagueCommand { LeagueId = request.Id },
                context.CancellationToken);

            return league.ToFullResponse();
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

    public override async Task<Empty> DeleteLeague(
        LeagueRequest request, ServerCallContext context)
    {
        try
        {
            await _deleteLeague.Handle(
                new DeleteLeagueCommand { LeagueId = request.Id },
                context.CancellationToken);

            return new Empty();
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    public override async Task<MatchResponse> GetMatchRequest(
        MatchRequest request, ServerCallContext context)
    {
        try
        {
            var match = await _getMatch.Handle(
                new GetMatchCommand { MatchId = request.Id },
                context.CancellationToken);

            return match.ToResponse();
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

    public override async Task<Empty> GenerateTimeTable(GenerateTimeTableRequest request, ServerCallContext context)
    {
        try
        {
            await _generateTimeTable.HandleAsync(new GenerateTimeTableCommand { LeagueId = request.LeagueId });

        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
        }

        return new Empty();
    }
}
