using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Единица компиляции
    /// </summary>
    public class CompilationUnit : AstNode
    {
        public CompilationUnit(ILineInfo li) : base(li)
        {
            TypeEntities = new List<TypeEntity>();
            Namespaces = new HashSet<string>();
        }

        public HashSet<string> Namespaces { get; }

        public List<TypeEntity> TypeEntities { get; }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            foreach (var entity in TypeEntities)
            {
                visitor.Visit(entity);
            }
        }
    }

    public class Root : AstNode, IScoped
    {
        public Root() : base(null)
        {
            CompilationUnits = new List<CompilationUnit>();
        }

        public List<CompilationUnit> CompilationUnits { get; }


        public override void Accept<T>(IVisitor<T> visitor)
        {
            foreach (var cu in CompilationUnits)
            {
                visitor.Visit(cu);
            }
        }

        public SymbolTable SymbolTable { get; set; }
    }
}