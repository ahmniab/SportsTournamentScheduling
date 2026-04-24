using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace STS.Resources.Domain.Entities;

[Index(nameof(LeagueId))]
public class Stadium
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public required string Name { get; set; }
    public string? Logo { get; set; }
    
    [JsonIgnore] 
    public virtual League League { get; set; }
}