using System;

namespace RemoteBrowser.Contracts.Messages
{
    public record BrowsingTaskCommand
    {
        public Guid TaskId { get; init; }
        public string Url { get; init; } = null!;
    }
}
