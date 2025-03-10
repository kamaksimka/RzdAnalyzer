using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using RZD.Common.Configs;
using RZD.Database;
using RZD.Scheduler.Jobs;

IHost host = Host.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration(app =>
      {
          app.AddJsonFile("appsettings.json");
      })
    .ConfigureServices((builder, services) =>
    {
        services.Configure<RzdConfig>(builder.Configuration.GetSection(RzdConfig.Section));
        services.AddDbContextFactory<DataContext>();

        var rzdConfig = builder.Configuration.GetSection(RzdConfig.Section).Get<RzdConfig>()!;

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("TrainsJob");

            q.AddJob<TrainsJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("TrainsJobTrigger")
                .WithCronSchedule(rzdConfig.TrainsJobSchedule)); 
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();


host.Run();
