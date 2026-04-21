namespace STS.Resources.Application.DTOs;
public class CreateLeagueDto
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } 
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; } 
}