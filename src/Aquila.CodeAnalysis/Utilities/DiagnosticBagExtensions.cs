using Devsense.PHP.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Errors;
using Aquila.Syntax;
using Aquila.Syntax.Text;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Pchp.CodeAnalysis
{
    internal static class DiagnosticBagExtensions
    {
        public static Location GetLocation(this SyntaxTree tree, LangElement expr) =>
            tree.GetLocation(expr.Span.ToTextSpan());

        public static void Add(
            this DiagnosticBag diagnostics,
            SourceRoutineSymbol routine,
            LangElement syntax,
            ErrorCode code,
            params object[] args)
        {
            Debug.Assert(syntax != null);
            Add(diagnostics, routine, syntax.Span.ToTextSpan(), code, args);
        }

        public static void Add(
            this DiagnosticBag diagnostics,
            SourceRoutineSymbol routine,
            TextSpan span,
            ErrorCode code,
            params object[] args)
        {
            var tree = routine.ContainingFile.SyntaxTree;
            var location = new SourceLocation(tree, span);
            diagnostics.Add(location, code, args);
        }

        public static void Add(this DiagnosticBag diagnostics, Location location, ErrorCode code, params object[] args)
        {
            var diag = MessageProvider.Instance.CreateDiagnostic(code, location, args);
            diagnostics.Add(diag);
        }

        public static Diagnostic ParserDiagnostic(SourceRoutineSymbol routine, Span span,
            Aquila.Syntax.Errors.ErrorInfo info, params string[] argsOpt)
        {
            return ParserDiagnostic(routine.ContainingFile.SyntaxTree, span, info, argsOpt);
        }

        public static Diagnostic ParserDiagnostic(SyntaxTree tree, Span span, Aquila.Syntax.Errors.ErrorInfo info,
            params string[] argsOpt)
        {
            ParserMessageProvider.Instance.RegisterError(info);

            return ParserMessageProvider.Instance.CreateDiagnostic(
                info.Severity == Aquila.Syntax.Errors.ErrorSeverity.WarningAsError,
                info.Id,
                new SourceLocation(tree, span.ToTextSpan()),
                argsOpt);
        }
    }
}