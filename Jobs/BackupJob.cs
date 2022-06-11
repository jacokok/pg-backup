using System.Text;
using CliWrap;
using Microsoft.Extensions.Options;
using Quartz;

namespace PGBackup.Jobs;

[DisallowConcurrentExecution]
public class BackupJob : IJob
{
    private readonly ILogger<TestJob> _logger;
    private readonly DBConfig _config;

    public BackupJob(ILogger<TestJob> logger, IOptions<DBConfig> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Job started");

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        var backupCommand = Cli
            .Wrap("docker")
            .WithArguments(new[] {
                "run", "-it", "-e", $"PGPASSWORD={_config.Password}", "--rm", "docker.io/postgres",
                "pg_dump", "-h", _config.Host, "-p", _config.Port, "-U", _config.Username, "-Fc", "-v", _config.Database
            })
            .WithEnvironmentVariables(env => env
                .Set("PGPASSWORD", "postgres")
            )
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
             | PipeTarget.ToFile($"./backup/backup-{DateTime.Now:yyyyMMdd-HHmmss}.dump");

        var result = await backupCommand.ExecuteAsync();

        _logger.LogInformation("Result: {result}", stdOutBuffer.ToString());
        _logger.LogInformation("Error: {error}", stdErrBuffer.ToString());
        _logger.LogInformation("Time: {RunTime}", result.RunTime);

        await Task.Delay(2000);
        _logger.LogInformation("Jobs done!");
    }
}