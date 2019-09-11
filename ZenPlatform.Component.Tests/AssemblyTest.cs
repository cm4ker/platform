using System;
using System.Collections.Generic;
using Xunit;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Sessions;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Component.Tests
{
    public class AssemblyTest
    {
        [Fact]
        public void BuildAstTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            IAssemblyPlatform pl = new DnlibAssemblyPlatform();
            var server = pl.CreateAssembly("Server");
            var client = pl.CreateAssembly("Client");

            var rootServer = new Root(null, new List<CompilationUnit>());
            var rootClient = new Root(null, new List<CompilationUnit>());

            foreach (var component in conf.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    new StagedGeneratorAst(component).StageServer(type, rootServer);
                    new StagedGeneratorAst(component).StageClient(type, rootClient);
                }
            }

            AstScopeRegister.Apply(rootServer);
            AstScopeRegister.Apply(rootClient);

            foreach (var cu in rootServer.Units)
            {
                var generator = new Generator(new GeneratorParameters(cu, server, CompilationMode.Server));
                generator.Build();
            }

            foreach (var cu in rootClient.Units)
            {
                var generator = new Generator(new GeneratorParameters(cu, client, CompilationMode.Client));
                generator.Build();
            }

            server.Write("Server.bll");
            client.Write("Client.bll");
        }
    }
}