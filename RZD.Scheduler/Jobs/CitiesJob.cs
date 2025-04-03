using Microsoft.Extensions.Options;
using Quartz;
using RZD.Application.Services;
using RZD.Common.Configs;

namespace RZD.Scheduler.Jobs
{
    public class CitiesJob : IJob
    {
        private readonly IntegrationService integrationService;
        private readonly RzdConfig rzdConfig;

        public CitiesJob(IntegrationService integrationService, IOptions<RzdConfig> options)
        {
            this.integrationService = integrationService;
            rzdConfig = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (rzdConfig.ExecuteCitiesJob)
            {
                await integrationService.RefreshCitiesAsync();
            }
        }
    }
}
