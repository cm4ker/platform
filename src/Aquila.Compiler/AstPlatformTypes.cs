using Aquila.Compiler.Contracts;
using Aquila.Core;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;

namespace Aquila.Compiler
{
    public static class AstPlatformTypes
    {
        public static void System(Root root, ITypeSystem ts)
        {
            var sb = ts.GetSystemBindings();

            var cu = new CompilationUnit(null, new UsingList(), new EntityList(), new NamespaceDeclarationList());
            var ns = new NamespaceDeclaration(null, "System", null, new EntityList(), null);

            cu.NamespaceDeclarations.Add(ns);
        }
    }
}