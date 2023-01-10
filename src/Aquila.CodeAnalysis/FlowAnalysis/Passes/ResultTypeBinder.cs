using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    /// <summary>
    /// Walks all expressions and resolves their access, operator method, and result CLR type.
    /// </summary>
    internal class ResultTypeBinder : GraphExplorer<TypeSymbol>
    {
        public AquilaCompilation DeclaringCompilation { get; }

        #region Initialization

        public ResultTypeBinder(AquilaCompilation compilation)
        {
            DeclaringCompilation = compilation ?? throw ExceptionUtilities.ArgumentNull(nameof(compilation));
        }

        public void Bind(SourceMethodSymbolBase method) => method.ControlFlowGraph?.Accept(this);

        public void Bind(SourceParameterSymbol parameter) => parameter.Initializer?.Accept(this);

        public void Bind(SourceFieldSymbol field) => field.Initializer?.Accept(this);

        #endregion
    }
}