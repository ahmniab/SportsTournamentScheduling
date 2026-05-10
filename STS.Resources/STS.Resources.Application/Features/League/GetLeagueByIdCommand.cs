namespace STS.Resources.Application.Features.League;

public class GetLeagueByIdCommand
{
    public string Id { get; set; } = string.Empty;
    public LeagueIncludeOptions? IncludeOptions { get; set; }
}
