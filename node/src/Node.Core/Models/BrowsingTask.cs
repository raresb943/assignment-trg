namespace Node.Core.Models;

public class BrowsingTask
{
    public Guid TaskId { get; set; }
    public required string Url { get; set; }
}

public class BrowsingResult
{
    public Guid TaskId { get; set; }
    public required string HtmlContent { get; set; }
}

public class PayloadResponse
{
    public required string Html { get; set; }
    public byte[]? Screenshot { get; set; }
    public int Status { get; set; }
}
