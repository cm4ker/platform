using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Platform
{
    public class XCCompiller: IXCCompiller
    {

        public XCCompiller()
        {

        }

        public IAssembly Build(XCRoot configuration, CompilationMode mode)
        {
            
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var assemblyBuilder = pl.CreateAssembly($"{configuration.ProjectName}{Enum.GetName(mode.GetType(), mode)}" );


            var root = new Root(null, new List<CompilationUnit>());

            foreach (var component in configuration.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    new StagedGeneratorAst(component).StageServer(type, root);
                }
            }

            AstScopeRegister.Apply(root);

            foreach (var cu in root.Units)
            {
                var generator = new Generator(new GeneratorParameters(cu, assemblyBuilder, mode));
                generator.Build();
            }

            return assemblyBuilder;
        }


        public string Build(XCRoot root, string outputDirectory, string buildName)
        {

          
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly(buildName);

            //STAGE0
            foreach (var t in root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage0(t, asm);
            }

            var list = new Dictionary<XCObjectTypeBase, IType>();

            //STAGE1
            foreach (var t in root.Data.ComponentTypes)
            {
                var b = t.Parent.ComponentImpl.Generator.Stage1(t, asm);
                list.Add(t, b);
            }

            //STAGE2
            foreach (var t in root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage2(t, (ITypeBuilder) list[t], list.ToImmutableDictionary(), asm);
            }
            var buildFIlePath = Path.Combine(outputDirectory, $"{buildName}.dll");
            asm.Write(buildFIlePath);

            return buildFIlePath;
            

        }
    }
}