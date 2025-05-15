namespace Node.Core.Models;

public class ContainerSettings
{
    public string PayloadImage { get; set; } = "payload:latest";
    public int PayloadPort { get; set; } = 3000;
    public TimeSpan InitializationTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
