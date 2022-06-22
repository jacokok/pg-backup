using System.Text;
using CliWrap;
using Microsoft.Extensions.Options;
using Quartz;

namespace PGBackup.Jobs;

[DisallowConcurrentExecution]
public class BackupJob : IJob
{
    private readonly ILogger<BackupJob> _logger;
    private readonly DBConfig _config;

    public BackupJob(ILogger<BackupJob> logger, IOptions<DBConfig> config)
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
            .Wrap("pg_dump")
            .WithArguments(new[] {
                "-h", _config.Host, "-p", _config.Port, "-U", _config.Username, "-Fc", "-v", _config.Database
            })
            .WithEnvironmentVariables(env => env
                .Set("PGPASSWORD", "postgres")
            )
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
             | PipeTarget.ToFile($"./backup/backup-{DateTime.Now:yyyyMMdd-HHmmss}.dump");

        var result = await backupCommand.ExecuteAsync();

        if (result.ExitCode == 0)
        {
            _logger.LogInformation("It was success");
        }
        else
        {
            _logger.LogInformation("It was failure");
        }

        _logger.LogInformation("Result: {result}", stdOutBuffer.ToString());
        _logger.LogInformation("Error: {error}", stdErrBuffer.ToString());
        _logger.LogInformation("Time: {RunTime}", result.RunTime);

        await Task.Delay(2000);
        _logger.LogInformation("Jobs done!");
    }
}