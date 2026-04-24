using System.Text.Json.Serialization;

namespace STS.Resources.Application.Models.Responses;

public class TimeSlotSummaryResponse
{
    public string Id;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StartTime;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EndTime;
}