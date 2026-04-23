namespace STS.Resources.Application.Features.Stadium;

public class UpdateStadiumCommand
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Logo { get; set; }
}
