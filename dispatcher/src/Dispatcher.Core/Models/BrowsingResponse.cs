using System;

namespace Dispatcher.Core.Models;

public class BrowsingResponse
{
    public Guid TaskId { get; set; }
    public string? HtmlContent { get; set; }
    public string? Status { get; set; } // "Processing", "Completed", "Failed"
    public string? Message { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Error { get; set; }
    public bool Success => string.IsNullOrEmpty(Error);
}
