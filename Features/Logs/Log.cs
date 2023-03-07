namespace PGBackup.Features.Logs;

public class Log
{
    public int Id { get; set; }
    public string? RenderedMessage { get; set; }
    public string? Level { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Properties { get; set; }
}