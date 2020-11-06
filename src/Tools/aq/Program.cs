using System;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;

namespace Aquila.Tools
{
    [Subcommand(typeof(DeployCommand), typeof(BuildCommand))]
    internal class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
           
        }
    }

    [Command("deploy")]
    public class DeployCommand
    {
        public void OnExecute()
        {
            Console.WriteLine("deploy!");
        }
    }

    [Command("build")]
    public class BuildCommand
    {
        public void OnExecute()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                 }
            };

            
            process.Start();
            
            string line = process.StandardOutput.ReadToEnd();
            Console.WriteLine(line);
            process.WaitForExit();
            
            Console.Write(process.ExitCode);
        }
    }

    [Command("migrate")]
    public class MigrateCommand
    {
    }

    [Command("rollback")]
    public class RollbackCommand
    {
    }
}