using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Domain.Entities;

[Index(nameof(LeagueId))]

public class Team
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public virtual League League { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public String? LogoUrl { get; set; }
}