using Humanizer;
using Quartz;
using Quartz.Impl.Matchers;

namespace PGBackup.Features.Jobs;

public class Response
{
    public string Name { get; set; } = "";
    public string Interval { get; set; } = "";
    public string Next { get; set; } = "";
    public string Last { get; set; } = "";
    public bool IsRunning { get; set; }
}

public class GetJobs : EndpointWithoutRequest<List<Response>>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public GetJobs(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Configure()
    {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        List<Response> result = new();
        IScheduler? scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        if (scheduler is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        var runningJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
        foreach (var jobKey in jobKeys)
        {
            Response item = new();
            var detail = await scheduler.GetJobDetail(jobKey, cancellationToken);
            if (detail != null)
            {
                item.Name = detail.Key.Name;
                foreach (var runningJob in runningJobs)
                {
                    if (runningJob.JobDetail.Key == detail.Key)
                    {
                        item.IsRunning = true;
                    }
                }
            }
            var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);
            foreach (var trigger in triggers)
            {
                if (trigger != null)
                {
                    item.Next = trigger.GetNextFireTimeUtc()?.Humanize() ?? "";
                    item.Last = trigger.GetPreviousFireTimeUtc()?.Humanize() ?? "";
                    var simpleTrigger = trigger as ISimpleTrigger;
                    item.Interval = simpleTrigger?.RepeatInterval.Humanize() ?? "";
                }
            }
            result.Add(item);
        }
        await SendAsync(result, cancellation: cancellationToken);
    }
}