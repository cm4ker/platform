using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Aquila.Core.Contracts;
using Aquila.Data;
using Aquila.Data.Tools;
using Aquila.Initializer;
using McMaster.Extensions.CommandLineUtils;

namespace Aquila.Tools
{
    [Subcommand(
        typeof(DeployCommand),
        typeof(BuildCommand),
        typeof(MigrateCommand),
        typeof(CreateCommand))]
    internal class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
        }
    }

    [Command("create")]
    public class CreateCommand
    {
        [Option("-s|--server", "Database server", CommandOptionType.SingleValue)]
        public string Server { get; }

        [Option("-t|--type", "Type of database within will be create solution", CommandOptionType.SingleValue)]
        public SqlDatabaseType DatabaseType { get; }

        [Option("-d|--database", "Database", CommandOptionType.SingleValue)]
        public string Database { get; }

        [Option("-u|--username", "User name", CommandOptionType.SingleValue)]
        public string Username { get; }

        [Option("-p|--password", "Password", CommandOptionType.SingleValue)]
        public string Password { get; }

        [Option("--port ", "Database server port", CommandOptionType.SingleValue)]
        public int Port { get; }

        [Option("-cs|--connectionString", "Connection string", CommandOptionType.SingleValue)]
        public string ConnectionString { get; }

        [Option("-c|--create", "Create database if not exists", CommandOptionType.NoValue)]
        public bool Create { get; }

        public void OnExecute()
        {
            var cstr = "";

            if (string.IsNullOrEmpty(ConnectionString))
            {
                var cb = new UniversalConnectionStringBuilder(DatabaseType);

                cb.Database = Database;
                cb.Server = Server;
                cb.Password = Password;
                cb.Username = Username;
                cb.Port = Port;

                cstr = cb.GetConnectionString();
            }
            else
            {
                cstr = ConnectionString;
            }

            if (DatabaseHelper.Exists(cstr, DatabaseType))
            {
                MigrationRunner.Migrate(cstr, DatabaseType);
            }
            else throw new Exception("Database not found");

            Console.WriteLine($"Done!");
        }
    }

    [Command("deploy")]
    public class DeployCommand
    {
        /// <summary>
        /// Folder with compiled executables
        /// </summary>
        [Option(ShortName = "pkg", Description = "Package path")]
        public string PackagePath { get; set; }

        [Option(ShortName = "e", Description = "Destination server for deploy")]
        public string Endpoint { get; set; }

        [Option(ShortName = "i", Description = "Instance name")]
        public string Instance { get; set; }

        public async Task OnExecuteAsync()
        {
            Action<string> dpl = (s) => Console.WriteLine(s);

            PackagePath = PackagePath.Replace("\"", "");

            dpl(@"Starting deploy...");

            //Step 1. We need executable solution folder
            dpl($"Package = {PackagePath}");
            dpl($"Endpoint = {Endpoint}");
            dpl($"Instance = {Instance}");

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

            // using var package = ZipFile.Open(PackagePath, ZipArchiveMode.Read);
            //TODO: add manifest to the archive

            // var totalCrc = new StringBuilder();
            // foreach (var entry in package.Entries)
            // {
            //     totalCrc.Append(entry.Crc32);
            // }
            //
            // var md5 = CreateChecksum(totalCrc.ToString());
            // dpl(@$"Checksum: {md5}");

            var deployUri = $"https://{Endpoint}/api/{Instance}/deploy";
            dpl($"Starting deploy package to {deployUri}...");

            //TODO: remove ugly hack!
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;

            var client = new HttpClient(handler);

            if (!File.Exists(PackagePath))
            {
                dpl("Package file not found");
                return;
            }


            var packageStream = File.OpenRead(PackagePath);

            var response = await client.PostAsync(deployUri, new StreamContent(packageStream));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dpl("OK!");
            }
            else
            {
                dpl($"Error: {response.StatusCode} : {response.ReasonPhrase}");
            }

            dpl(@"Done!");
        }

        public static string CreateChecksum(string input)
        {
            // Use input string to calculate MD5 hash
            using (SHA1 md5 = SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
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
        [Option(ShortName = "e", Description = "Destination server for deploy")]
        public string Endpoint { get; set; }


        [Option(ShortName = "i", Description = "Instance name")]
        public string Instance { get; set; }

        public async Task OnExecuteAsync()
        {
            Action<string> dpl = (s) => Console.WriteLine(s);

            //TODO: remove ugly hack!
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;

            var client = new HttpClient(handler);

            var deployUri = $"https://{Endpoint}/api/{Instance}/migrate";
            dpl($"Starting migrate package on {deployUri}...");
            var response = await client.PostAsync(deployUri, new StringContent(""));


            if (response.StatusCode == HttpStatusCode.OK)
            {
                dpl("OK!");
            }
        }
    }

    [Command("rollback")]
    public class RollbackCommand
    {
    }
}