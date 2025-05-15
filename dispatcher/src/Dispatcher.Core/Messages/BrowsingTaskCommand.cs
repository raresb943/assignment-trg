using System;

namespace Dispatcher.Core.Messages
{
    public record BrowsingTaskCommand
    {
        public Guid TaskId { get; init; }
        public string Url { get; init; } = null!;
    }
}
