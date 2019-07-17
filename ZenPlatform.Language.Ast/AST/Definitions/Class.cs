using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public class Class : TypeEntity
    {
        /// <summary>
        /// Create a module object.
        /// </summary>
        public Class(ILineInfo li, TypeBody typeBody, string name) : base(li, typeBody)
        {
            Name = name;
        }


        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitClass(this);
        }
    }
}