using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Test;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Test.Tools;
using Root = ZenPlatform.Language.Ast.Definitions.Root;


namespace ZenPlatform.Component.Tests
{
    public class AssemblyTest
    {
        private Assembly _clientAsm;
        private Assembly _serverAsm;


        private ITestOutputHelper _testOutput;

        public AssemblyTest(ITestOutputHelper testOutput)
        {
            Build();
            _testOutput = testOutput;
        }

        public void Build()
        {
            var conf = ConfigurationFactory.Create();
            IAssemblyPlatform pl = new DnlibAssemblyPlatform();

            var server = pl.CreateAssembly("Server");
            var client = pl.CreateAssembly("Client");

            var rootServer = new Root(null, new CompilationUnitList());
            var rootClient = new Root(null, new CompilationUnitList());

            foreach (var type in conf.TypeManager.Types.Where(x => x.IsObject))
            {
                new EntityPlatformGenerator(type.GetComponent()).StageServer(type, rootServer, SqlDatabaseType.SqlServer);
                new EntityPlatformGenerator(type.GetComponent()).StageClient(type, rootClient, SqlDatabaseType.SqlServer);
            }

            AstScopeRegister.Apply(rootServer);
            AstScopeRegister.Apply(rootClient);


            var genS = new Generator(new GeneratorParameters(rootServer.Units, server, CompilationMode.Server,
                SqlDatabaseType.SqlServer, conf));
            genS.Build();

            var genC = new Generator(new GeneratorParameters(rootClient.Units, client, CompilationMode.Client,
                SqlDatabaseType.SqlServer, conf));
            genC.Build();


            server.Write("Server.bll");
            //client.Write("Client.bll");

            var execDir = AppDomain.CurrentDomain.BaseDirectory;


            _serverAsm = Assembly.LoadFile(Path.Combine(execDir, "Server.bll"));
            // _clientAsm = Assembly.LoadFile(Path.Combine(execDir, "Client.bll"));
        }


        [Fact]
        public void TestManagerCreate()
        {
            var manager = _serverAsm.GetType("Entity.InvoiceManager");
            var facMethod = manager.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(facMethod);

            var invoice = facMethod.Invoke(null, new object[] { });
            Assert.NotNull(invoice);

            var it = invoice.GetType();
            var idProp = it.GetProperty("Id");

            Assert.NotNull(idProp);

            Assert.NotEqual(Guid.Empty, idProp.GetValue(invoice));
        }

        [Fact]
        public void TestObjectCreateAndSet()
        {
            var service = TestEnvSetup.GetServerServiceWithDatabase(_testOutput);
            var we = service.GetRequiredService<IWorkEnvironment>();
            var a = we.CreateSession(new PlatformUser() {Name = "User"});

            we.Initialize(new StartupConfig
            {
                DatabaseType = SqlDatabaseType.SqlServer,
                ConnectionString =
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            });

            ContextHelper.SetContext(new PlatformContext(a));

            //install session

            var cmd = _serverAsm.GetType("Entity.__cmd_HelloFromServer");
            var method = cmd.GetMethod("GetUserNameServer");

            var result = method.Invoke(null, null);
            Assert.NotNull(result);
        }
    }
}