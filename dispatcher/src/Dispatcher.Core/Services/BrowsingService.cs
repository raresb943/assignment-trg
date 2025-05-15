using Dispatcher.Core.Interfaces;
using Dispatcher.Core.Models;
using Microsoft.Extensions.Logging;
using RemoteBrowser.Contracts.Messages;
using System;

namespace Dispatcher.Core.Services;

public class BrowsingService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly ILogger<BrowsingService> _logger;

    public BrowsingService(
        ITaskRepository taskRepository,
        IMessageBusService messageBusService,
        ILogger<BrowsingService> logger)
    {
        _taskRepository = taskRepository;
        _messageBusService = messageBusService;
        _logger = logger;
    }

    public async Task<BrowsingResponse> ProcessBrowsingRequestAsync(BrowsingRequest request, CancellationToken cancellationToken = default)
    {
        var task = new BrowsingTask
        {
            Url = request.Url,
            Status = BrowsingTaskStatus.Pending
        };

        await _taskRepository.CreateAsync(task, cancellationToken);
        _logger.LogInformation("Created browsing task {TaskId} for URL {Url}", task.Id, task.Url); try
        {
            task.Status = BrowsingTaskStatus.InProgress;
            await _taskRepository.UpdateAsync(task, cancellationToken);

            await _messageBusService.SendBrowsingTaskCommand(new BrowsingTaskCommand
            {
                TaskId = task.Id,
                Url = task.Url
            });

            // send "pending" response, the client will poll for the result
            _logger.LogInformation("Sent browsing task {TaskId} to processing queue", task.Id);

            return new BrowsingResponse
            {
                TaskId = task.Id,
                Status = "Processing",
                Message = "Your request is being processed. Please check back later for results."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process browsing task {TaskId}", task.Id);
            task.Status = BrowsingTaskStatus.Failed;
            await _taskRepository.UpdateAsync(task, cancellationToken);
            throw;
        }
    }

    public async Task<BrowsingTask?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving browsing task {TaskId}", taskId);
            return await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve browsing task {TaskId}", taskId);
            throw;
        }
    }
}
