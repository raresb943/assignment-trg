using Dispatcher.Core.Models;

namespace Dispatcher.Core.Interfaces;

public interface INodeCommunicationService
{
    Task<string> SendBrowsingTaskToNodeAsync(BrowsingTask task, CancellationToken cancellationToken = default);
}
