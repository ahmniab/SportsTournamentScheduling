using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class TeamDto
{
    public string Id { get; set; } = string.Empty;
    public string LeagueId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    public static TeamDto From(TeamResponse r) => new()
    {
        Id = r.Id,
        LeagueId = r.LeagueId,
        Name = r.Name,
        LogoUrl = r.HasLogoUrl ? r.LogoUrl : null,
    };
}
