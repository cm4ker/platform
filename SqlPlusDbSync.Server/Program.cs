using System;
using System.Data.SqlClient;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Shared;
using Topshelf;

namespace SqlPlusDbSync.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();
            Logger.LogInfo("Starting application...");

            Config.Instance.Load();

            HostFactory.Run(x =>
            {
                x.Service<ServerWebService>(serv =>
                {
                    serv.ConstructUsing(name => new ServerWebService());
                    serv.WhenStarted(tc => tc.OnStart());
                    serv.WhenStopped(tc => tc.OnStop());
                });

                x.RunAsNetworkService();
                x.StartAutomatically();
                x.SetDescription("Tradepharm exchange server");
                x.SetDisplayName("SqlPlusDbSync.Server");
                x.SetServiceName("SqlPlusDbSync.Server");
            });

        }
    }
}
