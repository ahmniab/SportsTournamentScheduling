using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class TeamSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }

    public static TeamSummaryDto From(TeamSummary s) => new()
    {
        Id = s.Id,
        Name = s.HasName ? s.Name : null,
        LogoUrl = s.HasLogoUrl ? s.LogoUrl : null,
    };
}

public class StadiumSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Logo { get; set; }

    public static StadiumSummaryDto From(StadiumSummary s) => new()
    {
        Id = s.Id,
        Name = s.HasName ? s.Name : null,
        Logo = s.HasLogo ? s.Logo : null,
    };
}

public class TimeSlotSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }

    public static TimeSlotSummaryDto From(TimeSlotSummary s) => new()
    {
        Id = s.Id,
        StartTime = s.HasStartTime ? s.StartTime : null,
        EndTime = s.HasEndTime ? s.EndTime : null,
    };
}
