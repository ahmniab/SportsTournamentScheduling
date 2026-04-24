using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 

namespace STS.Resources.Application.Models.Responses;

public class LeagueResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime StartDate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LogoUrl { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<TeamSummaryResponse>? Teams { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<StadiumSummaryResponse>? Stadiums { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<TimeSlotSummaryResponse>? TimeSlots { get; set; }
}