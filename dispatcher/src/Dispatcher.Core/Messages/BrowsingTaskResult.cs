using System;

namespace Dispatcher.Core.Messages
{
    public record BrowsingTaskResult
    {
        public Guid TaskId { get; init; }
        public string HtmlContent { get; init; } = null!;
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
