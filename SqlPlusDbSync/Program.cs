using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using SqlPlusDbSync.Configuration;
using Topshelf;
using Logger = SqlPlusDbSync.Shared.Logger;

namespace SqlPlusDbSync.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            Logger.InitLogger();
            Logger.LogInfo("Starting application");

            Config.Instance.Load();
            HostFactory.Run(x =>
            {
                x.Service<ClientWindowsService>(serv =>
                {
                    serv.ConstructUsing(name => new ClientWindowsService());
                    serv.WhenStarted(tc => tc.OnStart());
                    serv.WhenStopped(tc => tc.OnStop());
                });

                x.RunAsNetworkService();
                x.StartAutomatically();
                x.SetDescription("Tradepharm exchange client");
                x.SetDisplayName("SqlPlusDbSync.Client");
                x.SetServiceName("SqlPlusDbSync.Client");
            });
        }

    }
}
