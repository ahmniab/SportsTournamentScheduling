namespace STS.Resources.Application.Features.Team;

public class CreateTeamCommand
{
    public string LeagueId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
