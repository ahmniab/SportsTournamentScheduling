namespace STS.BFF.API.Dtos;

public class CreateLeagueDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; }
}
