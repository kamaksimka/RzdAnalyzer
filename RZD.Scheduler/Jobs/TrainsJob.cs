using Quartz;

namespace RZD.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class TrainsJob: IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
