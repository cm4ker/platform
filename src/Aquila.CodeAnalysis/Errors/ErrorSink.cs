using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;

namespace Aquila.CodeAnalysis.Errors
{
    /// <summary>
    /// Stores errors from PHP parser.
    /// </summary>
    internal class ErrorSink : IErrorSink<Span>
    {
        readonly AquilaSyntaxTree _syntaxTree;

        List<Diagnostic> _diagnostics;

        public ErrorSink(AquilaSyntaxTree syntaxTree)
        {
            _syntaxTree = syntaxTree;
        }

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics != null ? _diagnostics.ToImmutableArray() : ImmutableArray<Diagnostic>.Empty; // Save an allocation if no errors were found

        public void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            if (info == FatalErrors.ParentAccessedInParentlessClass)
            {
                // ignore PHP2070: we'll handle it more precisely, also we might recover from it
                // otherwise we'll report it again in our diagnostics
                return;
            }

            if (_diagnostics == null)
            {
                _diagnostics = new List<Diagnostic>();
            }

            _diagnostics.Add(DiagnosticBagExtensions.ParserDiagnostic(_syntaxTree, span, info, argsOpt));
        }
    }
}
