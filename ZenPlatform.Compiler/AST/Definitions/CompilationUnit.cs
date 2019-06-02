using System.Collections.Generic;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
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

        public override void Accept(IVisitor visitor)
        {
            foreach (var entity in TypeEntities)
            {
                visitor.Visit(entity);
            }
        }
    }
}