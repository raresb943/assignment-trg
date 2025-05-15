using Dispatcher.Core.Models;
using Dispatcher.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Dispatcher.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrowsingController : ControllerBase
{
    private readonly BrowsingService _browsingService;
    private readonly ILogger<BrowsingController> _logger;

    public BrowsingController(BrowsingService browsingService, ILogger<BrowsingController> logger)
    {
        _browsingService = browsingService;
        _logger = logger;
    }
    [HttpPost]
    public async Task<ActionResult<BrowsingResponse>> BrowseSite([FromBody] BrowsingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received request to browse URL: {Url}", request.Url);
            var response = await _browsingService.ProcessBrowsingRequestAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to browse URL {Url}", request.Url);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request");
        }
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<BrowsingResponse>> GetBrowsingResult(Guid taskId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received request to get result for task: {TaskId}", taskId);
            var task = await _browsingService.GetTaskByIdAsync(taskId, cancellationToken);

            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found");
            }

            var response = new BrowsingResponse
            {
                TaskId = task.Id,
                Status = task.Status.ToString(),
                HtmlContent = task.Result,
                CompletedAt = task.CompletedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve task result {TaskId}", taskId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the result");
        }
    }
}
