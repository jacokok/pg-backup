using PGBackup.Features.Backup;
using Quartz;

namespace PGBackup.Features.Cloud.UploadBackup;

public class UploadEndpoint : Endpoint<Request, object>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public UploadEndpoint(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Configure()
    {
        Post("/cloud/backup/{FileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
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
                if (job.JobDetail.Key.Name == "Upload")
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
            JobDataMap jobDataMap = new()
            {
                { "fileName", request.FileName }
            };
            await scheduler.TriggerJob(new JobKey("Upload"), jobDataMap, cancellationToken);
            await SendNoContentAsync(cancellationToken);
        }
    }
}