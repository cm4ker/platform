using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract class TypeEntity : AstNode, IAstSymbol
    {
        /// <summary>
        /// Имя типа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тело типа
        /// </summary>
        public TypeBody TypeBody;

        protected TypeEntity(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public SymbolType SymbolType => SymbolType.Type;
    }
}