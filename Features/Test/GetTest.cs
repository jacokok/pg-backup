using Quartz;
using Quartz.Impl.Matchers;

namespace PGBackup.Features.Test;

public class GetTest : EndpointWithoutRequest<object>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public GetTest(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Configure()
    {
        Get("/test");
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

        // var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
        string result = "running: ";
        // foreach (var jobKey in jobKeys)
        // {

        // }

        if (runningJobs.Count > 0)
        {
            foreach (var job in runningJobs)
            {
                result += job.JobDetail.Key;
            }
        }
        else
        {
            await scheduler.TriggerJob(new JobKey("Test"), cancellationToken);
            result = "started job";
        }
        await SendStringAsync(result, cancellation: cancellationToken);
    }
}