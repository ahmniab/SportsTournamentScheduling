namespace STS.Resources.Application.Features.TimeSlot;

public class CreateTimeSlotCommand
{
    public string LeagueId { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
}
