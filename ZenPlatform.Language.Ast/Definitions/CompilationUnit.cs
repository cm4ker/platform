using System.Collections.Generic;
using System.Collections.Immutable;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Единица компиляции
    /// </summary>
    public partial class CompilationUnit : SyntaxNode
    {
        public static CompilationUnit Empty =>
            new CompilationUnit(null, new List<UsingBase>(), new List<TypeEntity>(), new List<NamespaceDeclaration>());

        public void AddEntity(TypeEntity type)
        {
            this.Entityes.Add(type);
            Childs.Add(type);
        }

        public void AddNsDecl(NamespaceDeclaration ns)
        {
            this.NamespaceDeclarations.Add(ns);
            Childs.Add(ns);
        }
    }
}