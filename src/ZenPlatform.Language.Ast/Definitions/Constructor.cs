using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Constructor
    {
        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public RBlockBuilder Builder;

        public SymbolType SymbolType => SymbolType.Constructor;

        public SymbolScopeBySecurity SymbolScope { get; set; }

        public static Constructor Default => new Constructor(null, new Block(new StatementList()),
            new ParameterList(), new AttributeList(), null);
    }
}