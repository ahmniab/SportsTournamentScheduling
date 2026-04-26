using System.Text.Json.Serialization;
using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class LeagueDto
{
    public string Id { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime StartDate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LogoUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TeamSummaryDto>? Teams { get; set; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<StadiumSummaryDto>? Stadiums { get; set; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TimeSlotSummaryDto>? TimeSlots { get; set; } = [];

    public static LeagueDto From(LeagueResponse r) => new()
    {
        Id = r.Id,
        OwnerId = r.OwnerId,
        Name = r.Name,
        CreatedAt = r.CreatedAt.ToDateTime(),
        StartDate = r.StartDate.ToDateTime(),
        LogoUrl = r.HasLogoUrl ? r.LogoUrl : null,
        Teams = r.Teams.Any() ? r.Teams.Select(TeamSummaryDto.From).ToList() : null,
        Stadiums = r.Stadiums.Any()? r.Stadiums.Select(StadiumSummaryDto.From).ToList() : null,
        TimeSlots = r.TimeSlots.Any()? r.TimeSlots.Select(TimeSlotSummaryDto.From).ToList() : null,
    };
}
