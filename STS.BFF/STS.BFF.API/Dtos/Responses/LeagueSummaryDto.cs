using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class LeagueSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; }

    public static LeagueSummaryDto From(LeagueResponseSummary r) => new()
    {
        Id = r.Id,
        OwnerId = r.OwnerId,
        Name = r.Name,
        CreatedAt = r.CreatedAt.ToDateTime(),
        StartDate = r.StartDate.ToDateTime(),
        LogoUrl = r.HasLogoUrl ? r.LogoUrl : null,
    };
}
