namespace Node.Core.Models;

public class UrlProcessingResult
{
    public required string HtmlContent { get; set; }
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}
