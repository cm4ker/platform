using System;
using System.Threading;
using Quartz;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client
{
    [DisallowConcurrentExecution]
    public class SimpleJob : IJob
    {
        public SimpleJob()
        {

        }

        public void Execute(IJobExecutionContext eContext)
        {

            Logger.Log.Info("Executing job");
            Console.WriteLine("Executing job");
            using (var client = new RabbitMQClient.Client(
                Config.Instance.MessageServer,
                Config.Instance.MessageServerPort,
                Config.Instance.MaxMessageSizeInKB,
                Config.Instance.MessageServerUserName,
                Config.Instance.MessageServerUserPassword))
            {

                client.Connect();

                if (!client.IsConnected) throw new Exception("Connection failure");

                using (AsnaDatabaseContext context = new AsnaDatabaseContext())
                {
                    Core c = new Core(context);
                    c.Init();

                    foreach (var pointId in context.LocatedPoints)
                    {
                        if (pointId != null)
                        {
                            client.Subscribe(pointId.ToString());
                            client.SendMessage(CommonHelper.InfoChannelName, new InfoConnectMessage() { From = pointId });
                        }
                    }

                    var responce = client.SendMessageWithResponceWait<RulesResponceMessage>(CommonHelper.ServerChannelName,
                        new RulesRequestMessage(context.GeneralPoint.ToString()), Config.Instance.TimeoutInSeconds);
                    if (responce == null)
                        return;
                    client.SendSplittedMessage(CommonHelper.ServerChannelName,
                        new VersionRequestMessage(context.LocatedPoints));
                }
                var seconds = 0;
                while (client.Subscribes.Count > 0 || client.ActiveTasks > 0)
                {
                    Thread.Sleep(1000);
                    seconds++;
                    if (seconds >= Config.Instance.TimeoutInSeconds)
                        break;
                }
            }
        }
    }
}