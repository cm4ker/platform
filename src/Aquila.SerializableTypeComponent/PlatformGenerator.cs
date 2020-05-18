using System;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Aquila.SerializableTypeComponent.Compilation;
using Aquila.Shared.Tree;

namespace Aquila.SerializableTypeComponent
{
    internal class PlatformGenerator : IPlatformGenerator
    {
        private readonly IComponent _component;

        public PlatformGenerator(IComponent component)
        {
            _component = component;
        }

        public void StageServer(IPType ipType, Node root)
        {
            var r = root as Root ?? throw new Exception("You must pass Root node to the generator");

            var cu = new CompilationUnit(null, new UsingList(), new EntityList(),
                new NamespaceDeclarationList());

            var ns = new NamespaceDeclaration(null, "SerializableTypes",
                new UsingList(), new EntityList(), new NamespaceDeclarationList());

            cu.NamespaceDeclarations.Add(ns);
            ns.AddEntity(new TypeTask(ipType, CompilationMode.Server, _component, ipType.Name));

            r.Units.Add(cu);
        }

        public void StageClient(IPType ipType, Node root)
        {
        }

        public void StageUI(IPType ipType, Node uiNode)
        {
        }

        public void StageGlobalVar(IGlobalVarManager manager)
        {
        }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm, Node task)
        {
            if (task is IInternalGenTask egt)
                return egt.Stage0(asm);
            else
                throw new Exception("Component doesn't support this task type");
        }

        public void Stage1(Node astTree, RoslynTypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode,
            IEntryPointManager sm)
        {
            if (!(astTree is IInternalGenTask gt)) throw new Exception("Not supported");
            
            gt.Stage1(builder, dbType, sm);
        }

        public void Stage2(Node astTree, RoslynTypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
        }

        public void StageInfrastructure(RoslynAssemblyBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
        }
    }
}