namespace STS.Resources.Application.Features.League;
public class UpdateLeagueCommand
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; }
}