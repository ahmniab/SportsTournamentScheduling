namespace STS.TimeTables.Application.Features.LeagueCommands;

public class SaveTimeTableCommand
{
    public Guid LeagueId { get; set; }
    public string LeagueJobId { get; set; } =  string.Empty;
}