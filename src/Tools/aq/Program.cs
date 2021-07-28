using System;
using System.Diagnostics;
using System.IO.Compression;
using Aquila.Core.Contracts;
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
        /// <summary>
        /// Folder with compiled executables
        /// </summary>
        [Argument(0, "pkg", description: "Package path")]
        public string PackagePath { get; set; }

        [Argument(1, "endpoint", description: "Destination server for deploy")]
        public string Endpoint { get; set; }

        public void OnExecute()
        {
            Action<string> dpl = (s) => Console.WriteLine(s);

            dpl(@"Starting deploy...");

            //Step 1. We need executable solution folder
            dpl($"Package = {PackagePath}");
            dpl($"Endpoint = {Endpoint}");
            dpl($"Deployer runtime version = {typeof(ILink).Assembly.GetName().Version}");

            /*
             Steps for success deploy
             
                 1) Check package
                 2) Check connection
                 3) Check platform runtime version
                 4) Read all metadata?
                 5) Create migration table for metadata (descriptors)
                 6) Migrate data
                 7) Update migration table
             */

            using var package = ZipFile.Open(PackagePath, ZipArchiveMode.Read);
            //TODO: add manifest to the archive

            dpl(@"Done!");
        }
    }

    [Command("build")]
    public class BuildCommand
    {
        public void OnExecute()
        {
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