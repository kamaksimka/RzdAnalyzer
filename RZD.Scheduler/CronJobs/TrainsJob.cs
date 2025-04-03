using Quartz;
using RZD.Application.Services;

namespace RZD.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class TrainsJob: IJob
    {
        private readonly IntegrationService integrationService;

        public TrainsJob(IntegrationService integrationService)
        {
            this.integrationService = integrationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await integrationService.RefreshTrainsAsync();
        }
    }
}
