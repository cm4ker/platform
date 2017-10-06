using System;
using System.Reflection;
using RabbitMQClient;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.Migrations;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Server
{
    public class ServerWebService
    {
        private Client client;

        public ServerWebService()
        {
            client = new Client(
                Config.Instance.MessageServer,
                Config.Instance.MessageServerPort,
                Config.Instance.MaxMessageSizeInKB,
                Config.Instance.MessageServerUserName,
                Config.Instance.MessageServerUserPassword);
        }

        public void OnStart()
        {
            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                Runner.MigrateToLatest(context.Connection.ConnectionString);

                try
                {
                    Console.Title =
                        $"ver {Assembly.GetExecutingAssembly().GetName().Version}. Server starting on server {context.Connection.DataSource}\\{context.Connection.Database}";
                }
                catch
                {
                    // ignored
                }


                Core c = new Core(context, true);
                c.LoadRulesFromFile("rules.dbe");
                c.Init();

                EntityGenerator eg = new EntityGenerator();
                eg.Generate(c);
            }

            client.Connect();
            client.Subscribe(CommonHelper.ServerChannelName);
        }

        public void OnStop()
        {
            client.Dispose();
        }
    }
}