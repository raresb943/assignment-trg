using Node.Core.Models;

namespace Node.Core.Interfaces;

public interface IPayloadService
{
    Task<string> ProcessUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<UrlProcessingResult> ProcessUrlWithStatusAsync(string url, string? containerId = null, CancellationToken cancellationToken = default);
}
