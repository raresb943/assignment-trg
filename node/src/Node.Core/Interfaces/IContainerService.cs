namespace Node.Core.Interfaces;

public interface IContainerService
{
    Task<string> StartPayloadContainerAsync(CancellationToken cancellationToken = default);
    Task WaitForContainerInitializationAsync(string containerId, CancellationToken cancellationToken = default);
    Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken = default);
}
