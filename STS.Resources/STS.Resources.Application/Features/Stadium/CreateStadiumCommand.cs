namespace STS.Resources.Application.Features.Stadium;

public class CreateStadiumCommand
{
    public string LeagueId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Logo { get; set; }
}
