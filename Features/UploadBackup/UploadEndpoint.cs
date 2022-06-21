using PGBackup.Features.DeleteBackup;
using Quartz;

namespace PGBackup.Features.UploadBackup;

public class UploadEndpoint : Endpoint<Request, object>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public UploadEndpoint(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Configure()
    {
        Post("/backup/upload/{FileName}");
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
            JobDataMap jobDataMap = new()
            {
                { "fileName", request.FileName }
            };

            await scheduler.TriggerJob(new JobKey("Upload"), jobDataMap, cancellationToken);
            result = "started job";
        }
        await SendStringAsync(result, cancellation: cancellationToken);
    }
}