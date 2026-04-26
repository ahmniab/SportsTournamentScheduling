namespace STS.BFF.API.Dtos;

public class CreateTeamDto
{
    public Guid LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
