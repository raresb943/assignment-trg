using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Node.Core.Interfaces;
using Node.Core.Models;

namespace Node.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrowsingController : ControllerBase
{
    private readonly IPayloadService _payloadService;
    private readonly ILogger<BrowsingController> _logger;

    public BrowsingController(IPayloadService payloadService, ILogger<BrowsingController> logger)
    {
        _payloadService = payloadService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessBrowsingTask([FromBody] BrowsingTask request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received browsing task {TaskId} for URL {Url}", request.TaskId, request.Url);

            var htmlContent = await _payloadService.ProcessUrlAsync(request.Url, cancellationToken);

            _logger.LogInformation("Successfully processed browsing task {TaskId}", request.TaskId);

            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process browsing task {TaskId} for URL {Url}", request.TaskId, request.Url);
            return StatusCode(500, ex.Message);
        }
    }
}
