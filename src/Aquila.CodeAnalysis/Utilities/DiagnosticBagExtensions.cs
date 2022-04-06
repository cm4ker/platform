using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Symbols;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Aquila.CodeAnalysis
{
    internal static class DiagnosticBagExtensions
    {
        public static Location GetLocation(this SyntaxTree tree, AquilaSyntaxNode expr) =>
            tree.GetLocation(expr.Span);

        public static void Add(
            this DiagnosticBag diagnostics,
            SourceMethodSymbol method,
            AquilaSyntaxNode syntax,
            ErrorCode code,
            params object[] args)
        {
            Debug.Assert(syntax != null);
            Add(diagnostics, method, syntax.Span, code, args);
        }

        public static void Add(
            this DiagnosticBag diagnostics,
            SourceMethodSymbol method,
            TextSpan span,
            ErrorCode code,
            params object[] args)
        {
            var tree = method.SyntaxNode.SyntaxTree;
            var location = new SourceLocation(tree, span);
            diagnostics.Add(location, code, args);
        }

        public static void Add(this DiagnosticBag diagnostics, Location location, ErrorCode code, params object[] args)
        {
            var diag = MessageProvider.Instance.CreateDiagnostic(code, location, args);
            diagnostics.Add(diag);
        }

        /// <summary>
        /// Checks if given collection contains fatal errors.
        /// </summary>
        public static bool HasErrors(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        }
    }
}