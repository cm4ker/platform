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
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Compiler.Platform;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Test.Assemblies;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Logging;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Test.Environment;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Compiler;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Core.Language.QueryLanguage;

namespace ZenPlatform.Core.Test
{
    public delegate void InvokeInClientServerContextDelegate(ServiceProvider clientService,
        ServiceProvider serverSerice, ClientPlatformContext clientContext);

    public class ClientServerTestBase
    {
        public void InvokeInClientServerContext(InvokeInClientServerContextDelegate action)
        {
            var serverServices = Initializer.GetServerService();
            var clientServices = Initializer.GetClientService();


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

            action(clientServices, serverServices, platformClient);

            accessPoint.Stop();
        }
    }

    public class UnitTestServerClient : ClientServerTestBase
    {
        [Fact]
        public void Connecting()
        {
            for (int i = 0; i < 10; i++)
            {
                var serverServices = Initializer.GetServerService();
                var clientServices = Initializer.GetClientService();

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
            var serverServices = Initializer.GetServerService();
            var clientServices = Initializer.GetClientService();


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

            accessPoint.Stop();
        }

        [Fact]
        public void ConnectingAndLoginAndInvoke()
        {
            InvokeInClientServerContext((clientService, serverService, clientContext) =>
            {
                GlobalScope.Client = clientContext.Client;
                var cmdType = clientContext.MainAssembly.GetType("CompileNamespace.__cmd_HelloFromServer");
                var result = cmdType.GetMethod("ClientCallProc").Invoke(null, new object[] {10});
                Assert.Equal(11, result);
            });
        }


        [Fact]
        public void CompileAndLoadAssembly()
        {
            var compiller = new XCCompiller();

            var root = Tests.Common.Factory.CreateExampleConfiguration();

            var _assembly2 = compiller.Build(root, CompilationMode.Server);
            var _assembly = compiller.Build(root, CompilationMode.Client);

            if (File.Exists("server.bll"))
                File.Delete("server.bll");

            if (File.Exists("test.bll"))
                File.Delete("test.bll");

            _assembly2.Write("server.bll");
            _assembly.Write("test.bll");

            Assert.Equal(_assembly.Name,
                $"{root.ProjectName}{Enum.GetName(typeof(Compiler.CompilationMode), Compiler.CompilationMode.Client)}");

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
            var manager = new AssemblyManager(new XCCompiller(), storage, new SimpleConsoleLogger<AssemblyManager>());

            var root = Tests.Common.Factory.CreateExampleConfiguration();

            manager.CheckConfiguration(root);

            var assemblies = storage.GetAssemblies(root.GetHash());

            //��������� ��������� ������ 
            Assert.NotNull(assemblies.FirstOrDefault(a => a.ConfigurationHash == root.GetHash()
                                                          && a.Name ==
                                                          $"{root.ProjectName}{Enum.GetName(typeof(Compiler.CompilationMode), Compiler.CompilationMode.Client)}"));


            //��������� ��������� ������ 
            Assert.NotNull(assemblies.FirstOrDefault(a => a.ConfigurationHash == root.GetHash()
                                                          && a.Name ==
                                                          $"{root.ProjectName}{Enum.GetName(typeof(Compiler.CompilationMode), Compiler.CompilationMode.Server)}"));
        }

        [Fact]
        public void AssemblyManagerClientServiceTest()
        {
            var serverService = Initializer.GetServerService();

            var assemblyManagerClientService = serverService.GetRequiredService<IAssemblyManagerClientService>();

            var env = serverService.GetRequiredService<IWorkEnvironment>();

            var assemblies = new List<AssemblyDescription>()
            {
                new AssemblyDescription()
                {
                    Name =
                        $"{env.Configuration.ProjectName}{Enum.GetName(typeof(Compiler.CompilationMode), Compiler.CompilationMode.Client)}",
                    ConfigurationHash = "fake",
                    AssemblyHash = "fake"
                }
            };


            var result = assemblyManagerClientService.GetDiffAssemblies(assemblies);

            //���������� � �������� ��������� ������
            Assert.NotNull(result.FirstOrDefault(a => a.ConfigurationHash == env.Configuration.GetHash()
                                                      && a.Name ==
                                                      $"{env.Configuration.ProjectName}{Enum.GetName(typeof(Compiler.CompilationMode), Compiler.CompilationMode.Client)}"));


            //�� ���������� ��������� � �������� ��������� ������
            Assert.Null(result.FirstOrDefault(a => a.Type == AssemblyType.Server));


            //var clientService = Initializer.GetClientService();
        }
    }
}