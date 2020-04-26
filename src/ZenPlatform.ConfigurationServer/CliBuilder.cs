using McMaster.Extensions.CommandLineUtils;

namespace ZenPlatform.IdeIntegration.Server
{
    public class CliBuilder
    {
        /*
         * Для тулзы будут доступны следующие режимы
         * 
         * ideintegration connect server port
         *
         */

        public static int Build(params string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");

            return app.Execute(args);
        }
    }
}