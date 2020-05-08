using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.AST;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Единица компиляции
    /// </summary>
    public partial class CompilationUnit : SyntaxNode
    {
        public static CompilationUnit Empty =>
            new CompilationUnit(null, new UsingList(), new EntityList(), new NamespaceDeclarationList());

        public IEnumerable<TypeEntity> GetTypes()
        {
            foreach (var entity in Entityes)
            {
                yield return entity;
            }

            foreach (var ns in NamespaceDeclarations)
            {
                foreach (var nsEntitye in ns.Entityes)
                {
                    yield return nsEntitye;
                }
            }
        }
    }
}