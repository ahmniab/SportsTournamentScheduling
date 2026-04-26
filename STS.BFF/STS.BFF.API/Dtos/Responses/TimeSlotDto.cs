using STS.BFF.API.Grpc;

namespace STS.BFF.API.Dtos.Responses;

public class TimeSlotDto
{
    public string Id { get; set; } = string.Empty;
    public string LeagueId { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public static TimeSlotDto From(TimeSlotResponse r) => new()
    {
        Id = r.Id,
        LeagueId = r.LeagueId,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
    };
}
