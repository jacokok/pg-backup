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
        await Task.Delay(5000);
        _logger.LogInformation("Jobs done!");
    }
}