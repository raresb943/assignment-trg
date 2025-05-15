using System;
using System.Threading.Tasks;
using Dispatcher.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using RemoteBrowser.Contracts.Messages;

namespace Dispatcher.Core.Consumers
{
    public class BrowsingTaskResultConsumer : IConsumer<BrowsingTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<BrowsingTaskResultConsumer> _logger;

        public BrowsingTaskResultConsumer(ITaskRepository taskRepository, ILogger<BrowsingTaskResultConsumer> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BrowsingTaskResult> context)
        {
            try
            {
                var result = context.Message;
                _logger.LogInformation("Received browsing task result for task {TaskId}", result.TaskId); var task = await _taskRepository.GetTaskById(result.TaskId);
                if (task == null)
                {
                    _logger.LogWarning("Could not find task with ID {TaskId} for received result", result.TaskId);
                    return;
                }

                task.Status = result.Success ? Models.BrowsingTaskStatus.Completed : Models.BrowsingTaskStatus.Failed;
                task.Result = result.HtmlContent;
                task.CompletedAt = DateTime.UtcNow;

                await _taskRepository.UpdateTask(task);
                _logger.LogInformation("Task {TaskId} updated with result", result.TaskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming browsing task result");
                throw;
            }
        }
    }
}
