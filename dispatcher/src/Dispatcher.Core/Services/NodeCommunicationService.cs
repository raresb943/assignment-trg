using Dispatcher.Core.Interfaces;
using Dispatcher.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Dispatcher.Core.Services;

public class NodeCommunicationService : INodeCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NodeCommunicationService> _logger;

    public NodeCommunicationService(HttpClient httpClient, ILogger<NodeCommunicationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> SendBrowsingTaskToNodeAsync(BrowsingTask task, CancellationToken cancellationToken = default)
    {
        try
        {
            var nodeRequest = new
            {
                TaskId = task.Id,
                Url = task.Url
            };

            var content = new StringContent(
                JsonSerializer.Serialize(nodeRequest),
                Encoding.UTF8,
                "application/json");

            _logger.LogInformation("Sending browsing task {TaskId} to node for URL {Url}", task.Id, task.Url);
            var response = await _httpClient.PostAsync("api/browsing", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Received response from node for task {TaskId}", task.Id);
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send browsing task {TaskId} to node", task.Id);
            throw;
        }
    }
}
