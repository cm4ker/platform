using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Compiler.Tests
{
    public class PlatformsTest
    {
        private IXCRoot r = Factory.CreateExampleConfiguration();

        [Fact]
        void TestCompileAndInvoke()
        {
            var dnlib = new DnlibAssemblyPlatform();
            var cecil = new CecilAssemblyPlatform();

            XCCompiler cd = new XCCompiler(dnlib);
            XCCompiler cc = new XCCompiler(cecil);

            if (File.Exists("server.bll"))
                File.Delete("server.bll");

            if (File.Exists("client.bll"))
                File.Delete("client.bll");

            var clientDnlib = cd.Build(r, CompilationMode.Client, SqlDatabaseType.SqlServer);
            var clientCecil = cc.Build(r, CompilationMode.Client, SqlDatabaseType.SqlServer);

            var serverDnlib = cd.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);
            var serverCecil = cc.Build(r, CompilationMode.Server, SqlDatabaseType.SqlServer);


            clientDnlib.Write("clientDnlib.bll");
            clientCecil.Write("clientCecil.bll");

            serverDnlib.Write("serverDnlib.bll");
            serverCecil.Write("serverCecil.bll");


            var asm = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clientDnlib.bll"));

            var cmdType = asm.GetType("CompileNamespace.__cmd_HelloFromServer");
            Assert.Throws<PlatformNotInitializedException>(() =>
            {
                var result = cmdType.GetMethod("ClientCallProc")
                    .Invoke(null, BindingFlags.DoNotWrapExceptions, null, new object[] {10}, null);
            });
        }
        
    }
}