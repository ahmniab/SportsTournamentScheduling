using STS.Resources.Application.Features.League.Commands.PrepareLeague;

namespace STS.Resources.Application.Interfaces;

public interface ILeagueReadyPublisher
{
    Task PublishAsync(PrepareLeagueResult result, CancellationToken ct = default);
}