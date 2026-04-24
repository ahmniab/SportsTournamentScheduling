namespace STS.Resources.Application.Features.TimeSlot;

public class UpdateTimeSlotCommand
{
    public string Id { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
}
