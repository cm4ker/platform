using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Constructor
    {
        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public IEmitter Builder;

        public SymbolType SymbolType => SymbolType.Constructor;

        public SymbolScope SymbolScope { get; set; }
    }
}