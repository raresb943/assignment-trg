using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Node.Core.Interfaces;
using Node.Core.Models;

namespace Node.Core.Services;

public class DockerPayloadService : IPayloadService
{
    private readonly ILogger<DockerPayloadService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IContainerService _containerService;

    public DockerPayloadService(
        HttpClient httpClient,
        IContainerService containerService,
        ILogger<DockerPayloadService> logger)
    {
        _httpClient = httpClient;
        _containerService = containerService;
        _logger = logger;
    }
    public async Task<string> ProcessUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var result = await ProcessUrlWithStatusAsync(url, null, cancellationToken);
        return result.HtmlContent;
    }

    public async Task<UrlProcessingResult> ProcessUrlWithStatusAsync(string url, string? containerId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing URL {Url}", url);

        bool createdContainer = false;
        string containerIdToUse = containerId ?? "";

        try
        {
            if (string.IsNullOrEmpty(containerIdToUse))
            {
                containerIdToUse = await _containerService.StartPayloadContainerAsync(cancellationToken);
                createdContainer = true;
                _logger.LogInformation("Started payload container with ID {ContainerId}", containerIdToUse);

                await _containerService.WaitForContainerInitializationAsync(containerIdToUse, cancellationToken);
            }

            try
            {
                var payloadResponse = await SendRequestToPayloadAsync(url, cancellationToken);
                _logger.LogInformation("Successfully fetched HTML content for {Url}", url);

                return new UrlProcessingResult
                {
                    HtmlContent = payloadResponse.Html,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing URL {Url}", url);
                return new UrlProcessingResult
                {
                    HtmlContent = string.Empty,
                    Success = false,
                    ErrorMessage = $"Failed to process URL: {ex.Message}"
                };
            }
        }
        finally
        {
            if (createdContainer && !string.IsNullOrEmpty(containerIdToUse))
            {
                await _containerService.StopAndRemoveContainerAsync(containerIdToUse, cancellationToken);
                _logger.LogInformation("Stopped and removed payload container {ContainerId}", containerIdToUse);
            }
        }
    }

    private async Task<PayloadResponse> SendRequestToPayloadAsync(string url, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request to payload service for URL: {Url}", url);

        try
        {
            if (_httpClient.BaseAddress == null)
            {
                _logger.LogWarning("BaseAddress is null. Setting default URL to http://payload:3000/");
                _httpClient.BaseAddress = new Uri("http://payload:3000/");
            }

            var payloadUrl = new Uri(_httpClient.BaseAddress, "browse");
            _logger.LogInformation("Sending request to payload endpoint: {PayloadUrl}", payloadUrl);

            var content = new StringContent(
                JsonSerializer.Serialize(new { url }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(payloadUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Received response from payload service: {Length} characters", responseContent.Length);

            using JsonDocument doc = JsonDocument.Parse(responseContent);
            string html = doc.RootElement.GetProperty("html").GetString() ?? string.Empty;

            _logger.LogInformation("Successfully parsed HTML content from payload response");

            return new PayloadResponse
            {
                Html = html,
                Screenshot = null,
                Status = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error communicating with payload service for URL: {Url}", url);

            // fallback HTML error response
            var errorHtml = $"<!DOCTYPE html><html><head><title>Error</title></head><body><h1>Error Processing URL</h1><p>Failed to process {url}: {ex.Message}</p></body></html>";

            return new PayloadResponse
            {
                Html = errorHtml,
                Screenshot = null,
                Status = 500
            };
        }
    }
}
