using System;
using System.Reflection;
using Quartz;
using Quartz.Impl;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.Migrations;

namespace SqlPlusDbSync.Client
{
    public class ClientWindowsService
    {
        private IScheduler scheduler;

        public ClientWindowsService()
        {

        }

        public void OnStart()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            var job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("simpleJob")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1")
                .StartNow()
                .WithSimpleSchedule(x =>
                    x.WithIntervalInSeconds(Config.Instance.ScheduleMainTaskInSeconds)
                        .RepeatForever()).Build();
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                Runner.MigrateToLatest(context.Connection.ConnectionString);

                Core c = new Core(context);
                c.Init();
                try
                {
                    Console.Title =
                        $"ver {Assembly.GetExecutingAssembly().GetName().Version}. Client starting on server {context.Connection.DataSource}\\{context.Connection.Database}";
                }
                catch
                {
                    // ignored
                }
                EntityGenerator eg = new EntityGenerator();
                eg.Generate(c);
            }
        }

        public void OnStop()
        {
            scheduler.Shutdown();
        }
    }
}