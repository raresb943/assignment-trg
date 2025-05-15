using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Node.Core.Interfaces;
using Node.Core.Models;
using RemoteBrowser.Contracts.Messages;

namespace Node.Service.Consumers
{
    public class BrowsingTaskCommandConsumer : IConsumer<BrowsingTaskCommand>
    {
        private readonly ILogger<BrowsingTaskCommandConsumer> _logger;
        private readonly IContainerService _containerService;
        private readonly IPayloadService _payloadService;
        private readonly IBus _bus;

        public BrowsingTaskCommandConsumer(
            ILogger<BrowsingTaskCommandConsumer> logger,
            IContainerService containerService,
            IPayloadService payloadService,
            IBus bus)
        {
            _logger = logger;
            _containerService = containerService;
            _payloadService = payloadService;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<BrowsingTaskCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Received browsing task command for task {TaskId}", command.TaskId);

            try
            {
                _logger.LogInformation("Creating container for task {TaskId}", command.TaskId);
                var containerId = await _containerService.StartPayloadContainerAsync();

                _logger.LogInformation("Waiting for container {ContainerId} initialization", containerId);
                await _containerService.WaitForContainerInitializationAsync(containerId);
                _logger.LogInformation("Processing URL {Url} for task {TaskId}", command.Url, command.TaskId);
                var result = await _payloadService.ProcessUrlWithStatusAsync(command.Url, containerId);

                _logger.LogInformation("Releasing container {ContainerId} for task {TaskId}", containerId, command.TaskId);
                await _containerService.StopAndRemoveContainerAsync(containerId);
                await _bus.Publish(new BrowsingTaskResult
                {
                    TaskId = command.TaskId,
                    HtmlContent = result.HtmlContent,
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage
                });

                _logger.LogInformation("Completed processing task {TaskId}", command.TaskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing browsing task {TaskId}", command.TaskId);

                // Send failure result
                await _bus.Publish(new BrowsingTaskResult
                {
                    TaskId = command.TaskId,
                    HtmlContent = string.Empty,
                    Success = false,
                    ErrorMessage = $"Failed to process URL: {ex.Message}"
                });
            }
        }
    }
}
