
namespace STS.Resources.Application.Features.League;

public class CreateLeagueCommand 
{
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? LogoUrl { get; set; }
}