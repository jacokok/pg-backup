using Quartz;

namespace PGBackup.Features.Backup.CreateBackup;

public class CreateBackupEndpoint : EndpointWithoutRequest<object>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public CreateBackupEndpoint(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Configure()
    {
        Post("/backup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        if (scheduler is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        var runningJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);

        string result = "running: ";

        if (runningJobs.Count > 0)
        {
            foreach (var job in runningJobs)
            {
                result += job.JobDetail.Key;
            }
        }
        else
        {
            await scheduler.TriggerJob(new JobKey("Backup"), cancellationToken);
            result = "started job";
        }
        await SendStringAsync(result, cancellation: cancellationToken);
    }
}