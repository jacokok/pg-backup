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

        bool isAlreadyRunning = false;

        if (runningJobs.Count > 0)
        {
            foreach (var job in runningJobs)
            {
                if (job.JobDetail.Key.Name == "Backup")
                {
                    isAlreadyRunning = true;
                }
            }
        }

        if (isAlreadyRunning)
        {
            await SendNotFoundAsync(cancellationToken);
        }
        else
        {
            await scheduler.TriggerJob(new JobKey("Backup"), cancellationToken);
            await SendNoContentAsync(cancellationToken);
        }
    }
}