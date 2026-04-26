namespace STS.BFF.API.Dtos;

public class CreateTimeSlotDto
{
    public Guid LeagueId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
