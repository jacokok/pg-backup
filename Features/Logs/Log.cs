namespace PGBackup.Features.Logs;

public class Log
{
    public string Message { get; set; } = "";
    public int Level { get; set; }
    public DateTime Timestamp { get; set; }
}