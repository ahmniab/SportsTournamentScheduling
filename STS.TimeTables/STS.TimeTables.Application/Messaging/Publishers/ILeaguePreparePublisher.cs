namespace STS.TimeTables.Application.Messaging.Publishers;

public interface ILeaguePreparePublisher
{
    public Task PublishAsync(Guid leagueId,CancellationToken ct);
}