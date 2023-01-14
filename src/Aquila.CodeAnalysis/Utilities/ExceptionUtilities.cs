using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Semantics;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Compiler.Utilities;
using Aquila.Syntax.Ast;

namespace Aquila.CodeAnalysis.Utilities
{
    static class ExceptionUtilities
    {
        /// <summary>
        /// Gets <see cref="System.NotImplementedException"/> with aproximate location of the error.
        /// </summary>
        public static NotImplementedException NotImplementedException(this CodeGenerator cg, string message = null,
            IAquilaOperation op = null)
        {
            return NotImplementedException(cg.Builder, message, op: op, method: cg.Method,
                debugmethod: cg.DebugMethod);
        }

        /// <summary>
        /// Gets <see cref="System.NotImplementedException"/> with aproximate location of the error.
        /// </summary>
        public static NotImplementedException NotImplementedException(ILBuilder il, string message = null,
            IAquilaOperation op = null, SourceMethodSymbolBase method = null, MethodSymbol debugmethod = null)
        {
            string location = null;

            var syntax = op?.AquilaSyntax;
            if (syntax != null)
            {
                // get location from AST
                // var unit = syntax.SyntaxTree.Source;
                // unit.GetLineColumnFromPosition(syntax.Span.Start, out int line, out int col);
                // location = $"{unit.FilePath}({line + 1}, {col + 1})";
            }
            else if (il.SeqPointsOpt != null && il.SeqPointsOpt.Count != 0)
            {
                // get location from last sequence point
                var pt = il.SeqPointsOpt.Last();
                // ((AquilaSyntaxTree)pt.SyntaxTree).Source.GetLineColumnFromPosition(pt.Span.Start, out int line,
                //     out int col);
                // location = $"{pt.SyntaxTree.FilePath}({line + 1}, {col + 1})";
            }
            else if (method != null)
            {
                location = $"{method.Syntax.SyntaxTree.FilePath} in '{method.MethodName}'";
            }
            else if (debugmethod != null)
            {
                location = $"{debugmethod.ContainingType.GetFullName()}::{debugmethod.MethodName}";

                // if (debugmethod.ContainingType is SourceTypeSymbol srctype)
                // {
                //     // location = $"{srctype.ContainingFile.SyntaxTree.FilePath} in {location}";
                // }
            }
            else
            {
                location = "<unknown>";
            }

            //
            return new NotImplementedException($"{message} not implemented at {location}");
        }

        public static ArgumentNullException ArgumentNull(string argName)
        {
            return new ArgumentNullException(argName);
        }

        public static ArgumentNullException ArgumentNull()
        {
            return new ArgumentNullException();
        }

        public static InvalidOperationException UnexpectedValue(object o)
        {
            string output = string.Format("Unexpected value '{0}' of type '{1}'", o,
                (o != null) ? o.GetType().FullName : "<unknown>");
            Debug.Assert(false, output);

            // We do not throw from here because we don't want all Watson reports to be bucketed to this call.
            return new InvalidOperationException(output);
        }

        internal static InvalidOperationException Unreachable
        {
            get { return new InvalidOperationException("This program location is thought to be unreachable."); }
        }
    }
}