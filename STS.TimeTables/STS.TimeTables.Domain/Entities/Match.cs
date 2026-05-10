using Microsoft.EntityFrameworkCore;

namespace STS.TimeTables.Domain.Entities;

[Index(nameof(LeagueId))]
public class Match
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid Team1Id { get; set; }
    public Guid Team2Id { get; set; }
    public Guid TimeSlotId { get; set;}
    public Guid StadiumId { get; set; }
    public DateTime Date { get; set; }
    
    public virtual League? League { get; set; }
}