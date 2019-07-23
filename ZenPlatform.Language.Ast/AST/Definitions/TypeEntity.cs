using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract class TypeEntity : SyntaxNode, IAstSymbol
    {
        /// <summary>
        /// Имя типа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тело типа
        /// </summary>
        public TypeBody TypeBody { get; }

        protected TypeEntity(ILineInfo lineInfo, TypeBody tb) : base(lineInfo)
        {
            TypeBody = Children.SetSlot(tb, 0);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public SymbolType SymbolType => SymbolType.Type;
    }
}