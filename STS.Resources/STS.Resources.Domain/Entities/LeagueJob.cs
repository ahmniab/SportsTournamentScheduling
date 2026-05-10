
namespace STS.Resources.Domain.Entities;

public class LeagueJob
{
    public Guid LeagueId { get; set; }
    public LeagueJobStatus Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; } 
}
