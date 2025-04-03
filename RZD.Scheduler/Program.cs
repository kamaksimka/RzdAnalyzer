using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using RZD.Application.Services;
using RZD.Common.Configs;
using RZD.Database;
using RZD.Scheduler.Jobs;

IHost host = Host.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration(app =>
      {
          app.AddJsonFile("appsettings.json");
          app.AddEnvironmentVariables();
      })
    .ConfigureServices((builder, services) =>
    {
        services.Configure<RzdConfig>(builder.Configuration.GetSection(RzdConfig.Section));
        services.AddDbContextFactory<DataContext>();
        services.AddScoped<IntegrationService>();



        var rzdConfig = builder.Configuration.GetSection(RzdConfig.Section).Get<RzdConfig>()!;

        services.AddQuartz(q =>
        {
            var trainsJobKey = new JobKey("TrainsJob");

            q.AddJob<TrainsJob>(opts => opts.WithIdentity(trainsJobKey));

            q.AddTrigger(opts => opts
                .ForJob(trainsJobKey)
                .WithIdentity("TrainsJobTrigger")
                .WithCronSchedule(rzdConfig.TrainsJobSchedule));


            var citiesJobKey = new JobKey("CitiesJob");

            q.AddJob<CitiesJob>(opts => opts.WithIdentity(citiesJobKey));

            q.AddTrigger(opts => opts
                .ForJob(citiesJobKey)
                .WithIdentity("CitiesJobTrigger")
                .StartNow());
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();


// Выполнение миграций сразу после сборки и запуска приложения
using (var scope = host.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
    var dbContext = dbContextFactory.CreateDbContext();
    await dbContext.Database.MigrateAsync();
}

host.Run();
