using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
        typeof(MigrateCommand))]
    internal class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
        }
    }

    public class CreateCommand
    {
        [Option("--project_name", "Name of new project", CommandOptionType.SingleValue)]
        [Required]
        public string ProjectName { get; }

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

        [Option("--connString", "Connection string", CommandOptionType.SingleValue)]
        public string ConnectionString { get; }

        [Option("-c|--create", "Create database if not exists", CommandOptionType.NoValue)]
        public bool Create { get; }

        public void OnExecute()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                OnCreateDbCommand(ProjectName, DatabaseType, Server,
                    Port, Database, Username, Password,
                    Create);
            else
                OnCreateDbCommand(ProjectName, DatabaseType, ConnectionString, Create);
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType,
            UniversalConnectionStringBuilder stringBuilder, bool createIfNotExists)
        {
            var connectionString = stringBuilder.GetConnectionString();

            MigrationRunner.Migrate(connectionString, databaseType);

            var dataContext = new DataConnectionContext(databaseType, connectionString);

            Console.WriteLine($"Done!");
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string connectionString,
            bool createIfNotExists)
        {
            OnCreateDbCommand(projectName, databaseType,
                UniversalConnectionStringBuilder.FromConnectionString(databaseType, connectionString),
                createIfNotExists);
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string server, int port,
            string database, string userName, string password, bool createIfNotExists)
        {
            Console.WriteLine($"Start creating new project {projectName}");
            Console.WriteLine(
                $"DatabaseType: {databaseType}\nServer: {server}\nDatabase {database}\nUsername {userName}\nPassword {password}");
            var cb = new UniversalConnectionStringBuilder(databaseType);

            //После успешного созадания базы пробуем к ней подключиться, провести миграции и 
            //создать новую конфигурацию
            cb.Database = database;
            cb.Server = server;
            cb.Password = password;
            cb.Username = userName;
            cb.Port = port;

            Console.WriteLine(cb.GetConnectionString());

            OnCreateDbCommand(projectName, databaseType, cb, createIfNotExists);
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

        public void OnExecute()
        {
            Action<string> dpl = (s) => Console.WriteLine(s);

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

            var deployUri = $"http://{Endpoint}/api/{Instance}/deploy";
            dpl($"Starting deploy package to {deployUri}...");

            HttpWebRequest request = WebRequest.CreateHttp(deployUri);
            request.Method = WebRequestMethods.Http.Post;


            var reqStream = request.GetRequestStream();
            var packageStream = File.OpenRead(PackagePath);
            packageStream.CopyTo(reqStream);

            using HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                dpl("OK!");
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

        public void OnExecute()
        {
            Action<string> dpl = (s) => Console.WriteLine(s);

            var deployUri = $"http://{Endpoint}/api/{Instance}/migrate";
            dpl($"Starting migrate package on {deployUri}...");

            HttpWebRequest request = WebRequest.CreateHttp(deployUri);
            request.Method = WebRequestMethods.Http.Post;

            using HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
            if (responce.StatusCode == HttpStatusCode.OK)
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