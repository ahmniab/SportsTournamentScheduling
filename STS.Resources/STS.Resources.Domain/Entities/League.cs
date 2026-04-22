using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace STS.Resources.Domain.Entities;

[Index(nameof(OwnerId))]
public class League
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime StartDate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LogoUrl { get; set; }
}