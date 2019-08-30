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
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Platform
{
    public class XCCompiller: IXCCompiller
    {

        public XCCompiller()
        {

        }

        public IDictionary<string, Stream> Build(XCRoot root)
        {

            var clientName = $"{root.ProjectName}_client";
            var serverName = $"{root.ProjectName}_server";

            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var server = pl.CreateAssembly(serverName);
            var client = pl.CreateAssembly(clientName);

            var rootServer = new Root(null, new List<CompilationUnit>());
            var rootClient = new Root(null, new List<CompilationUnit>());

            foreach (var component in root.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    new StagedGeneratorAst(component).StageServer(type, rootServer);
                    new StagedGeneratorAst(component).StageServer(type, rootClient);
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

            var result = new Dictionary<string, Stream>();

            var clientStream = new MemoryStream();
            client.Write(clientStream);
            clientStream.Seek(0, SeekOrigin.Begin);
            result.Add(clientName, clientStream);

            var serverStream = new MemoryStream();
            server.Write(serverStream);
            serverStream.Seek(0, SeekOrigin.Begin);
            result.Add(serverName, serverStream);

            return result;
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