using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Environment;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ZenPlatform.Compiler.Platform;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using dnlib.DotNet;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Test.Assemblies;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Logging;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Test.Environment;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Core.Test.Logging;
using ZenPlatform.EntityComponent.Entity;

namespace ZenPlatform.Core.Test
{
    public delegate void InvokeInClientServerContextDelegate(ServiceProvider clientService,
        ServiceProvider serverSerice, ClientPlatformContext clientContext);

    public class UnitTestServerClient : ClientServerTestBase
    {
        private ITestOutputHelper _testOutput;

        public UnitTestServerClient(ITestOutputHelper testOutput) : base(testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void Connecting()
        {
            for (int i = 0; i < 1; i++)
            {
                var serverServices = Initializer.GetServerService(_testOutput);
                var clientServices = Initializer.GetClientService(_testOutput);

                var environmentManager = serverServices.GetRequiredService<IPlatformEnvironmentManager>();
                Assert.NotEmpty(environmentManager.GetEnvironmentList());

                var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
                accessPoint.Start();

                //need check listing

                IInvokeService s = null;

                var platformClient = clientServices.GetRequiredService<ClientPlatformContext>();
                platformClient.Connect(new Settings.DatabaseConnectionSettings()
                    {Address = "127.0.0.1:12345", Database = "Library"});
                //need check connection

                accessPoint.Stop();

                Task.Delay(1000).Wait();
            }
        }

        [Fact]
        public void ConnectingAndLogin()
        {
            var serverServices = Initializer.GetServerService(_testOutput);
            var clientServices = Initializer.GetClientService(_testOutput);


            var environmentManager = serverServices.GetRequiredService<IPlatformEnvironmentManager>();
            Assert.NotEmpty(environmentManager.GetEnvironmentList());


            var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
            accessPoint.Start();
            //need check listing

            var platformClient = clientServices.GetRequiredService<ClientPlatformContext>();
            platformClient.Connect(new Settings.DatabaseConnectionSettings()
                {Address = "127.0.0.1:12345", Database = "Library"});
            //need check connection

            platformClient.Login("admin", "admin");
            var assembly = platformClient.LoadMainAssembly();
            Assert.NotNull(assembly);


            //Task.Delay(1000000).Wait();
            accessPoint.Stop();
        }

        [Fact]
        public void ConnectingAndLoginAndInvoke()
        {
            for (int i = 0; i < 1; i++)
            {
                InvokeInClientServerContext((clientService, serverService, clientContext) =>
                {
                    GlobalScope.Client = clientContext.Client;

                    var cmdType = clientContext.MainAssembly.GetType("CompileNamespace.__cmd_HelloFromServer");

                    var result = cmdType.GetMethod("ClientCallProc")
                        .Invoke(null, new object[] {10});

                    Assert.Equal(11, result);

                    var userName = cmdType.GetMethod("GetUserNameServer")
                        .Invoke(null, new object[] { });

                    Assert.Equal("Anonymous", userName);
                });
            }
        }

        [Fact]
        public void GetViewBag()
        {
            InvokeInClientServerContext((clientService, serverService, clientContext) =>
            {
                GlobalScope.Client = clientContext.Client;

                var vb = clientContext.Client.Invoke<ViewBag>(new Route("Test_GetInvoice"));

                InvoiceLink il = new InvoiceLink(vb);

                Assert.Equal("Entity = (10:{8b888935-895d-4806-beaf-0f9e9217ad1b})", il.Presentation);

                var stLink = il.Store;

                Assert.Equal("Entity = (11:{9de86d2e-1597-4518-b24c-8bfe7f25bf50})", stLink.Presentation);
            });
        }


        [Fact]
        public void Test()
        {
            var module = ModuleDefMD.Load("ZenPlatform.Cli.dll");
        }

        [Fact]
        public void CompileAndLoadAssembly()
        {
            var compiller = new XCCompiler(new DnlibAssemblyPlatform());

            var root = Factory.CreateExampleConfiguration();

            var _assembly2 = compiller.Build(root, CompilationMode.Server, SqlDatabaseType.SqlServer);
            var _assembly = compiller.Build(root, CompilationMode.Client, SqlDatabaseType.SqlServer);

            if (File.Exists("server.bll"))
                File.Delete("server.bll");

            if (File.Exists("client.bll"))
                File.Delete("client.bll");

            _assembly2.Write("server.bll");
            _assembly.Write("client.bll");

            Assert.Equal(_assembly.Name,
                $"{root.ProjectName}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}");

            PlatformAssemblyLoadContext loadContext =
                new PlatformAssemblyLoadContext(new TestClientAssemblyManager(_assembly));

            var result = loadContext.LoadFromAssemblyName(new AssemblyName(_assembly.Name));

            Assert.NotNull(result);

            var invoice = result.CreateInstance("Documents._Invoice");
            Assert.NotNull(invoice);


            
        }

        [Fact]
        public void AssemblyManagerTest()
        {
            var storage = new TestAssemblyStorage();
            var manager =
                new AssemblyManager(new XCCompiler(new DnlibAssemblyPlatform()), storage,
                    new XUnitLogger<AssemblyManager>(_testOutput),
                    new XCConfManipulator());

            var root = Factory.CreateExampleConfiguration();

            if (manager.CheckConfiguration(root))
                manager.BuildConfiguration(root, SqlDatabaseType.SqlServer);

            var assemblies = storage.GetAssemblies(root.GetHash());

            Assert.NotNull(assemblies.FirstOrDefault(a => a.ConfigurationHash == root.GetHash()
                                                          && a.Name ==
                                                          $"{root.ProjectName}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}"));


            Assert.NotNull(assemblies.FirstOrDefault(a => a.ConfigurationHash == root.GetHash()
                                                          && a.Name ==
                                                          $"{root.ProjectName}{Enum.GetName(typeof(CompilationMode), CompilationMode.Server)}"));
        }

        [Fact]
        public void AssemblyManagerClientServiceTest()
        {
            var serverService = Initializer.GetServerService(_testOutput);

            var assemblyManagerClientService = serverService.GetRequiredService<IAssemblyManagerClientService>();
            var manipulator = serverService.GetRequiredService<IConfigurationManipulator>();
            var env = serverService.GetRequiredService<IWorkEnvironment>();

            env.Initialize(new StartupConfig());

            var assemblies = new List<AssemblyDescription>()
            {
                new AssemblyDescription()
                {
                    Name =
                        $"{env.Configuration.ProjectName}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}",
                    ConfigurationHash = "fake",
                    AssemblyHash = "fake"
                }
            };


            var result = assemblyManagerClientService.GetDiffAssemblies(assemblies);

            Assert.NotNull(result.FirstOrDefault(a => a.ConfigurationHash == manipulator.GetHash(env.Configuration)
                                                      && a.Name ==
                                                      $"{env.Configuration.ProjectName}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}"));


            Assert.Null(result.FirstOrDefault(a => a.Type == AssemblyType.Server));


            //var clientService = Initializer.GetClientService();
        }

       /*
        [Fact]
        public void MigrationTest()
        {
            var serverServices = Initializer.GetServerServiceWithDatabase(_testOutput);


            var environmentManager = serverServices.GetRequiredService<IPlatformEnvironmentManager>();
            Assert.NotEmpty(environmentManager.GetEnvironmentList());


            var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
            accessPoint.Start();
            //need check listing

        }
        */

    }
}