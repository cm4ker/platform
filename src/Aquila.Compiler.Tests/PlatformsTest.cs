using System;
using System.IO;
using System.Reflection;
using Xunit;
using Aquila.Compiler.Cecil;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Dnlib;
using Aquila.Compiler.Platform;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.QueryBuilder;
using Aquila.Test.Tools;

namespace Aquila.Compiler.Tests
{
    public class PlatformsTest
    {
        private IProject r = ConfigurationFactory.Create();

        [Fact]
        void CompilationTest()
        {
            var dnlib = new RoslynAssemblyPlatform();
            ;
            XCCompiler cd = new XCCompiler(dnlib);
            var asm = cd.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);

            if (File.Exists("server.bll"))
                File.Delete("server.bll");

            asm.Write("server.bll");

            Assert.True(true);
        }

        [Fact]
        void XamlTest()
        {
            // var dnlib = new DnlibAssemblyPlatform();
            // XCCompiler cd = new XCCompiler(dnlib);
            // var asm = cd.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);
            //
            // if (File.Exists("server.bll"))
            //     File.Delete("server.bll");
            //
            // asm.Write("server.bll");
            //
            // var loaded = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.bll"));
            //
            // var formType = loaded.GetType("Entity.StoreEditorForm");
            // var cmdType = loaded.GetType("Entity.__StoreEditorForm");
            //
            // var result = cmdType.GetMethod("Get")
            //     .Invoke(null, BindingFlags.DoNotWrapExceptions, null, new object[] { }, null);
            //
            // Assert.NotNull(result);
        }

        [Fact]
        void CompilationClientAndServerTest()
        {
            var dnlib = new RoslynAssemblyPlatform();
            XCCompiler cd = new XCCompiler(dnlib);
            var asm = cd.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);
            var asmClient = cd.Build(r, CompilationMode.Client, SqlDatabaseType.SqlServer);

            if (File.Exists("server.bll"))
                File.Delete("server.bll");

            asm.Write("server.bll");

            if (File.Exists("client.bll"))
                File.Delete("client.bll");

            asmClient.Write("client.bll");

            Assert.True(true);
        }

        [Fact]
        void TestCompileAndInvoke()
        {
            // var dnlib = new DnlibAssemblyPlatform();
            // var cecil = new CecilAssemblyPlatform();
            //
            // XCCompiler cd = new XCCompiler(dnlib);
            // XCCompiler cc = new XCCompiler(cecil);
            //
            // if (File.Exists("server.bll"))
            //     File.Delete("server.bll");
            //
            // if (File.Exists("client.bll"))
            //     File.Delete("client.bll");
            //
            // var clientDnlib = cd.Build(r, CompilationMode.Client, SqlDatabaseType.SqlServer);
            // var clientCecil = cc.Build(r, CompilationMode.Client, SqlDatabaseType.SqlServer);
            //
            // var serverDnlib = cd.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);
            // var serverCecil = cc.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);
            //
            //
            // clientDnlib.Write("clientDnlib.bll");
            // clientCecil.Write("clientCecil.bll");
            //
            // serverDnlib.Write("serverDnlib.bll");
            // serverCecil.Write("serverCecil.bll");
            //
            //
            // // var asm = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clientDnlib.bll"));
            // //
            // // var cmdType = asm.GetType("CompileNamespace.__cmd_HelloFromServer");
            // // Assert.Throws<PlatformNotInitializedException>(() =>
            // // {
            // //     var result = cmdType.GetMethod("ClientCallProc")
            // //         .Invoke(null, BindingFlags.DoNotWrapExceptions, null, new object[] {10}, null);
            // // });
        }
    }
}