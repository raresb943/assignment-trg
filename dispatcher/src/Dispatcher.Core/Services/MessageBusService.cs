using System;
using System.Threading.Tasks;
using Dispatcher.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using RemoteBrowser.Contracts.Messages;

namespace Dispatcher.Core.Services
{
    public class MessageBusService : IMessageBusService
    {
        private readonly IBus _bus;
        private readonly ILogger<MessageBusService> _logger;

        public MessageBusService(IBus bus, ILogger<MessageBusService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task SendBrowsingTaskCommand(BrowsingTaskCommand command)
        {
            try
            {
                _logger.LogInformation("Sending browsing task command for task {TaskId} to message bus", command.TaskId);
                await _bus.Publish(command);
                _logger.LogInformation("Successfully sent browsing task command for task {TaskId}", command.TaskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send browsing task command for task {TaskId}", command.TaskId);
                throw;
            }
        }
    }
}
