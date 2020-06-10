using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Compiler;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Compiler.Roslyn;
using Aquila.Core.Contracts;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions.Functions
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
                                      | ((IsClientCall) ? FunctionFlags.ServerClientCall : 0)
                                      | ((IsOperation) ? FunctionFlags.IsOperation : 0);

        private bool IsServer => Attributes.Any(x => x.Type.TypeName == "Server") || !Attributes.Any();
        private bool IsClient => Attributes.Any(x => x.Type.TypeName == "Client");
        private bool IsClientCall => Attributes.Any(x => x.Type.TypeName == "ClientCall");
        private bool IsOperation => Attributes.Any(x => x.Type.TypeName == "Operation");

        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public PCilBody Builder;

        private readonly Block _block;

        public bool IsPublic { get; set; }

        public bool IsGeneric => GenericParameters.Any();

        public SymbolType SymbolType => SymbolType.Method;
        public SymbolScopeBySecurity SymbolScope { get; set; }
    }
}