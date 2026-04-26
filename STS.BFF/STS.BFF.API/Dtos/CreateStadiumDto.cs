namespace STS.BFF.API.Dtos;

public class CreateStadiumDto
{
    public Guid LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Logo { get; set; }
}
