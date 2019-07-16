using ZenPlatform.Compiler.Contracts.Symbols;


namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Описывает модуль
    /// </summary>
    public class Module : TypeEntity
    {
        /// <summary>
        /// Create a module object.
        /// </summary>
        public Module(ILineInfo li, TypeBody typeBody, string name) : base(li)
        {
            TypeBody = typeBody;
            Name = name;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitModule(this);
        }
    }
}