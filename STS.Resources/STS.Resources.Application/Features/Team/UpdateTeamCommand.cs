namespace STS.Resources.Application.Features.Team;

public class UpdateTeamCommand
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
