using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Compiler;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Language.Ast.Definitions;


namespace ZenPlatform.EntityComponent.Entity
{
    public class NewPlatformGenerator : IPlatformGenerator
    {
        private readonly XCComponent _component;

        public NewPlatformGenerator(XCComponent component)
        {
            _component = component;
        }

        public void StageServer(XCObjectTypeBase type, Root root)
        {
            var cu = new CompilationUnit(null, new List<NamespaceBase>(),
                new List<TypeEntity>() {new ComponentClass(_component, type, null, type.Name)});

            root.Add(cu);
        }

        public void StageClient(XCObjectTypeBase type, Root root)
        {
            var cu = new CompilationUnit(null, new List<NamespaceBase>(),
                new List<TypeEntity>() {new ComponentClass(_component, type, null, type.Name)});

            root.Add(cu);
        }

        public void PatchType(ComponentAstBase astTree, ITypeBuilder builder)
        {
        }
    }
}