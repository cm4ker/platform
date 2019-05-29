using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public class TypeEntity : AstNode
    {
        /// <summary>
        /// Имя типа
        /// </summary>
        public string Name;

        /// <summary>
        /// Тело типа
        /// </summary>
        public TypeBody TypeBody;

        public TypeEntity(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public override void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}