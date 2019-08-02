using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract partial class TypeEntity
    {
        /// <summary>
        /// Тело типа
        /// </summary>
        public TypeBody TypeBody { get; }

        protected TypeEntity(ILineInfo lineInfo, string name, TypeBody tb) : this(lineInfo, name)
        {
            TypeBody = tb;
            Childs.Add(TypeBody);
        }

        public SymbolType SymbolType => SymbolType.Type;
    }
}