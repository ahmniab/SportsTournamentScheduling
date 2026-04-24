using System.Text.Json.Serialization;

namespace STS.Resources.Application.Models.Responses;

public class TeamSummaryResponse
{
    public string Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LogoUrl { get; set; }
}