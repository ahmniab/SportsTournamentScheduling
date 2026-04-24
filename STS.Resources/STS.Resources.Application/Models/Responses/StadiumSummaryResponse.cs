using System.Text.Json.Serialization;

namespace STS.Resources.Application.Models.Responses;

public class StadiumSummaryResponse
{
    public string Id;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LogoUrl;
}