namespace STS.TimeTables.Domain.Entities;

public class League
{
    public Guid Id { get; set; }
    public DateTime GeneratedAt { get; set; }
    public float BestFitness { get; set; }
    
    public virtual ICollection<Match>? Matches { get; set; }
}