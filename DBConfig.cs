namespace PGBackup;

public class DBConfig
{
    public string Host { get; set; } = "localhost";
    public string Port { get; set; } = "5432";
    public string Password { get; set; } = "postgres";
    public string Username { get; set; } = "postgres";
    public string Database { get; set; } = "postgres";
    public string LogsConnectionString { get; set; } = "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=logs";
    public string LogTable { get; set; } = "logs";
}