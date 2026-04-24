namespace STS.Resources.Application.Features.League;

public class GetLeagueByIdCommand
{
    public String Id { get; set; }
    public LeagueIncludeOptions? IncludeOptions { get; set; }
}
