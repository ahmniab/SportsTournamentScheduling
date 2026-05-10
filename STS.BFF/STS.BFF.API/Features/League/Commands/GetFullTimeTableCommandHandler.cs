using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using STS.BFF.API.Dtos.Responses;
using STS.BFF.API.Grpc;
using STS.TimeTables.API.Grpc;

namespace STS.BFF.API.Features.League.Commands;

public class GetFullTimeTableCommandHandler
{
    private readonly TimeTablesService.TimeTablesServiceClient _timeTablesService;
    private readonly LeagueService.LeagueServiceClient _leagueService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetFullTimeTableCommandHandler(
        TimeTablesService.TimeTablesServiceClient timeTablesService,
        LeagueService.LeagueServiceClient leagueService,
        IHttpContextAccessor httpContextAccessor)
    {
        _timeTablesService = timeTablesService;
        _leagueService = leagueService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FullTimeTableDto> HandleAsync(GetFullTimeTableCommand command)
    {
        var httpContext = _httpContextAccessor.HttpContext
                          ?? throw new UnauthorizedAccessException("HTTP context is not available.");

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (accessToken == null) throw new UnauthorizedAccessException("Access token is missing.");
        var headers = new Metadata { { "Authorization", $"Bearer {accessToken}" } };

        var timeTableRequest = new LeagueRequest { Id = command.LeagueId.ToString() };
        var timeTableResponse = await _timeTablesService.GetFullLeagueAsync(timeTableRequest, headers);

        var leagueIncludeOptions = new LeagueIncludeOptions
        {
            Teams = IncludeOption.IncludeAll,
            Stadiums = IncludeOption.IncludeAll,
            TimeSlots = IncludeOption.IncludeAll
        };

        var getLeagueRequest = new GetLeagueRequest
        {
            Id = command.LeagueId.ToString(),
            Include = leagueIncludeOptions
        };

        var leagueResponse = await _leagueService.GetLeagueAsync(getLeagueRequest, headers);
        var leagueDto = LeagueDto.From(leagueResponse);

        var result = FullTimeTableDto.From(timeTableResponse, leagueDto);

        return result;
    }
}