using System;
using System.Threading.Tasks;
using RemoteBrowser.Contracts.Messages;

namespace Dispatcher.Core.Interfaces
{
    public interface IMessageBusService
    {
        Task SendBrowsingTaskCommand(BrowsingTaskCommand command);
    }
}
