using System.Collections.Generic;
using System.Collections.Immutable;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Единица компиляции
    /// </summary>
    public class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ILineInfo li, List<string> namespaces, ImmutableList<TypeEntity> entityes) : base(li)
        {
            TypeEntities = entityes;
            Namespaces = new HashSet<string>(namespaces);


            var slot = 0;
            foreach (var ent in TypeEntities)
            {
                Children.SetSlot(ent, slot++);
            }
        }

        public HashSet<string> Namespaces { get; }

        public IReadOnlyCollection<TypeEntity> TypeEntities { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }
    }

    public class Root : SyntaxNode, IScoped
    {
        public Root() : base(null)
        {
            CompilationUnits = new List<CompilationUnit>();
        }

        public List<CompilationUnit> CompilationUnits { get; }


        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitRoot(this);
        }

        public SymbolTable SymbolTable { get; set; }
    }
}