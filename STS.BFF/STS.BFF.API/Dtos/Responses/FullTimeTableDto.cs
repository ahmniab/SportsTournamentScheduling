using System.Text.Json.Serialization;
using Google.Protobuf.WellKnownTypes;
using STS.BFF.API.Grpc;
using STS.TimeTables.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class FullTimeTableDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public float BestFitness { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LeagueDto? League { get; set; }

    public List<MatchDto> Matches { get; set; } = [];

    public static FullTimeTableDto From(
        GetFullLeagueResponse timeTableResponse,
        LeagueDto leagueData)
    {
        return new FullTimeTableDto
        {
            Id = timeTableResponse.Id,
            GeneratedAt = timeTableResponse.GeneratedAt.ToDateTime(),
            BestFitness = timeTableResponse.BestFitness,
            League = leagueData,
            Matches = timeTableResponse.Matches
                .Select(m => MatchDto.From(m, leagueData))
                .ToList()
        };
    }
}

public class MatchDto
{
    public string Id { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LeagueDto? League { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TeamSummaryDto? Team1 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TeamSummaryDto? Team2 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeSlotDto? TimeSlot { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StadiumDto? Stadium { get; set; }

    public DateTime Date { get; set; }

    public static MatchDto From(MatchResponse matchResponse, LeagueDto leagueData) => new()
    {
        Id = matchResponse.Id,
        League = leagueData,
        Team1 = leagueData.Teams?.FirstOrDefault(t => t.Id == matchResponse.Team1Id),
        Team2 = leagueData.Teams?.FirstOrDefault(t => t.Id == matchResponse.Team2Id),
        TimeSlot = leagueData.TimeSlots?.FirstOrDefault(ts => ts.Id == matchResponse.TimeSlotId)
            is TimeSlotSummaryDto summary ? new TimeSlotDto
            {
                Id = summary.Id,
                LeagueId = leagueData.Id,
                StartTime = summary.StartTime ?? string.Empty,
                EndTime = summary.EndTime ?? string.Empty,
            } : null,
        Stadium = leagueData.Stadiums?.FirstOrDefault(s => s.Id == matchResponse.StadiumId)
            is StadiumSummaryDto stadium ? new StadiumDto
            {
                Id = stadium.Id,
                LeagueId = leagueData.Id,
                Name = stadium.Name ?? string.Empty,
                Logo = stadium.Logo,
            } : null,
        Date = matchResponse.Date.ToDateTime(),
    };
}
