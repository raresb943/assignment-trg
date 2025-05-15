namespace Dispatcher.Core.Models;

public class BrowsingTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Result { get; set; }
    public DateTime? CompletedAt { get; set; }
    public BrowsingTaskStatus Status { get; set; } = BrowsingTaskStatus.Pending;
}

public enum BrowsingTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}
