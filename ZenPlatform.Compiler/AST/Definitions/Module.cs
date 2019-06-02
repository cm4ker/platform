using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(TypeBody);
        }
    }
}