using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Constructor
    {
        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public IEmitter Builder;


        public SymbolType SymbolType => SymbolType.Constructor;
    }
}