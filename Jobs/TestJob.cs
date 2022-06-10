using System.Text;
using CliWrap;
using Quartz;

namespace PGBackup.Jobs;

[DisallowConcurrentExecution]
public class TestJob : IJob
{
    private readonly ILogger<TestJob> _logger;

    public TestJob(ILogger<TestJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Job started");

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        var result = await Cli
            .Wrap("wsl")
            .WithArguments("docker exec postgres pg_dump --version")
            .WithWorkingDirectory("C:\\Windows\\System32")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync();

        _logger.LogInformation("Result: {result}", stdOutBuffer.ToString());
        _logger.LogInformation("Error: {error}", stdErrBuffer.ToString());
        _logger.LogInformation("Time: {RunTime}", result.RunTime);
        await Task.Delay(2000);
        _logger.LogInformation("Jobs done!");
    }
}