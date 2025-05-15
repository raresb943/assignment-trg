namespace Dispatcher.Core.Models;

public class BrowsingRequest
{
    public required string Url { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}
