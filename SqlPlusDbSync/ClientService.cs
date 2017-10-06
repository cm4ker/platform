using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Quartz;
using Quartz.Impl;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client
{
    partial class ClientService : ServiceBase
    {
        private IScheduler scheduler;

        public ClientService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            var job = JobBuilder.Create<SimpleJob>()
                                .WithIdentity("simpleJob")
                                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1")
                .StartNow()
                .WithSimpleSchedule(x =>
                    x.WithIntervalInMinutes(1)
                     .RepeatForever()).Build();
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

        protected override void OnStop()
        {
            scheduler.Shutdown();
        }
    }
}
