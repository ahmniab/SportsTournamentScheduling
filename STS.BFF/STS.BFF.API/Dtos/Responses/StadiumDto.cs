using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class StadiumDto
{
    public string Id { get; set; } = string.Empty;
    public string LeagueId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Logo { get; set; }

    public static StadiumDto From(StadiumResponse r) => new()
    {
        Id = r.Id,
        LeagueId = r.LeagueId,
        Name = r.Name,
        Logo = r.HasLogo ? r.Logo : null,
    };
}
