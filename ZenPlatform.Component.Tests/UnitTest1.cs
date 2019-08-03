using System;
using System.Collections.Generic;
using Xunit;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Component.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void BuildAstTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly("Debug");

            foreach (var component in conf.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    var root = new Root(null, new List<CompilationUnit>());
                    new StagedGeneratorAst(component).Stage0(type, root);

                    foreach (var cu in root.Units)
                    {
                        new Generator(new GeneratorParameters(cu, asm, CompilationMode.Server)).Build();
                    }
                }
            }

            asm.Write("Debug.bll");
        }
    }
}