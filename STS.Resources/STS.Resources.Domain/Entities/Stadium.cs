using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Domain.Entities;

[Index(nameof(LeagueId))]
public class Stadium
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public virtual League? League { get; set; }
    public required string Name { get; set; }
    public string? Logo { get; set; }
}