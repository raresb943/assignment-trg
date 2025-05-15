using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Node.Core.Interfaces;
using Node.Core.Models;

namespace Node.Core.Services;

public class DockerContainerService : IContainerService
{
    private readonly ILogger<DockerContainerService> _logger;
    private readonly HttpClient _httpClient;
    private readonly HttpClient _healthClient;
    private readonly ContainerSettings _settings;

    public DockerContainerService(
        HttpClient httpClient,
        IHttpClientFactory httpClientFactory,
        IOptions<ContainerSettings> options,
        ILogger<DockerContainerService> logger)
    {
        _httpClient = httpClient;
        _healthClient = httpClientFactory.CreateClient("ContainerHealthCheck");
        _settings = options.Value;
        _logger = logger;
    }
    public async Task<string> StartPayloadContainerAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting new payload container");

        var containerId = "payload"; // using the service name as defined in docker-compose

        try
        {
            _logger.LogInformation("Using payload container {ContainerId}", containerId);
            // simulate delay for container initialization
            await Task.Delay(100, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start payload container");
            throw;
        }

        return containerId;
    }

    public async Task WaitForContainerInitializationAsync(string containerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Waiting for container {ContainerId} to initialize", containerId);

        // Poll the container health
        var healthEndpoint = new Uri(_healthClient.BaseAddress!, "health");
        var maxRetries = 30;
        var retryDelayMs = 500;

        _logger.LogInformation("Checking health at {endpoint}", healthEndpoint);

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var response = await _healthClient.GetAsync(healthEndpoint, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Container {ContainerId} is ready", containerId);
                    return;
                }

                _logger.LogInformation("Health check attempt {attempt}/{maxRetries} for container {containerId}: {status}",
                    i + 1, maxRetries, containerId, response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Health check attempt {attempt}/{maxRetries} failed, retrying...", i + 1, maxRetries);
            }

            await Task.Delay(retryDelayMs, cancellationToken);
        }

        throw new TimeoutException($"Container {containerId} failed to initialize within timeout period");
    }

    public async Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Container {ContainerId} will remain running as a shared service", containerId);
        // Simulate stopping and removing the container
        await Task.CompletedTask;
    }
}
