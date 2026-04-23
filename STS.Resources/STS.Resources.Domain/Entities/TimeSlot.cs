using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Domain.Entities;

[Index(nameof(LeagueId))]
public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public required string StartTime { get; set; }
    public required string EndTime { get; set; }

    public virtual League? League { get; set; }
}