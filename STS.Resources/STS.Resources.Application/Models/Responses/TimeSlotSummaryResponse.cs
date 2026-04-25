using System.Text.Json.Serialization;

namespace STS.Resources.Application.Models.Responses;

public class TimeSlotSummaryResponse
{
    public string Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StartTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EndTime { get; set; }
}