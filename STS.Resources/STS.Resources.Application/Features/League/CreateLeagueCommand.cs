
namespace STS.Resources.Application.Features.League;

public class CreateLeagueCommand 
{
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; }
}