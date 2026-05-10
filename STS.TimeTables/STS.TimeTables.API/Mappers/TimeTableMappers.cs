using Google.Protobuf.WellKnownTypes;
using STS.TimeTables.API.Grpc;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.API.Mappers;

internal static class TimeTableMappers
{
    internal static GetLeagueSummaryResponse ToSummaryResponse(this League league)
        => new()
        {
            Id           = league.Id.ToString(),
            GeneratedAt  = Timestamp.FromDateTime(league.GeneratedAt.ToUniversalTime()),
            BestFitness  = league.BestFitness,
        };

    internal static GetFullLeagueResponse ToFullResponse(this League league)
    {
        var response = new GetFullLeagueResponse
        {
            Id          = league.Id.ToString(),
            GeneratedAt = Timestamp.FromDateTime(league.GeneratedAt.ToUniversalTime()),
            BestFitness = league.BestFitness,
        };

        response.Matches.AddRange(league.Matches?.Select(m => m.ToResponse()) ?? Enumerable.Empty<MatchResponse>());

        return response;
    }

    internal static MatchResponse ToResponse(this Match match)
        => new()
        {
            Id         = match.Id.ToString(),
            LeagueId   = match.LeagueId.ToString(),
            Team1Id    = match.Team1Id.ToString(),
            Team2Id    = match.Team2Id.ToString(),
            TimeSlotId = match.TimeSlotId.ToString(),
            StadiumId  = match.StadiumId.ToString(),
            Date       = Timestamp.FromDateTime(match.Date.ToUniversalTime()),
        };
}
