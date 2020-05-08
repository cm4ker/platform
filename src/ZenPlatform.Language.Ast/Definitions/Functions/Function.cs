using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    /// <summary>
    /// Describes a function.
    /// </summary>
    public partial class Function
    {
        private const int BLOCK_SLOT = 0;

        /// <summary>
        /// Флаги функции
        /// </summary>
        public FunctionFlags Flags => ((IsClient) ? FunctionFlags.Client : 0)
                                      | ((IsServer) ? FunctionFlags.Server : 0)
                                      | ((IsClientCall) ? FunctionFlags.ServerClientCall : 0);

        private bool IsServer => Attributes.Any(x => x.Type.TypeName == "Server") || !Attributes.Any();
        private bool IsClient => Attributes.Any(x => x.Type.TypeName == "Client");
        private bool IsClientCall => Attributes.Any(x => x.Type.TypeName == "ClientCall");

        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public RBlockBuilder Builder;

        private readonly Block _block;

        public bool IsPublic { get; set; }

        public bool IsGeneric => GenericParameters.Any();

        public SymbolType SymbolType => SymbolType.Method;
        public SymbolScopeBySecurity SymbolScope { get; set; }
    }
}