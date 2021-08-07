using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Table of local variables used within method.
    /// </summary>
    internal sealed partial class LocalsTable
    {
        #region Fields & Properties

        readonly Dictionary<VariableName, LocalVariableReference> /*!*/
            _dict = new Dictionary<VariableName, LocalVariableReference>();

        /// <summary>
        /// Enumeration of direct local variables.
        /// </summary>
        public IEnumerable<LocalVariableReference> Variables => _dict.Values;

        /// <summary>
        /// Count of local variables.
        /// </summary>
        public int Count => _dict.Count;

        /// <summary>
        /// Containing method. Cannot be <c>null</c>.
        /// </summary>
        public SourceMethodSymbol Method => _method;

        readonly SourceMethodSymbol _method;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes table of locals of given method.
        /// </summary>
        public LocalsTable(SourceMethodSymbol method)
        {
            Contract.ThrowIfNull(method);

            _method = method;

            //
            PopulateParameters();
        }

        #endregion

        void PopulateParameters()
        {
            // parameters
            foreach (var p in _method.SourceParameters)
            {
                _dict[new VariableName(p.Name)] = new ParameterReference(p, Method);
            }

            if (!_method.IsStatic)
            {
                _dict[VariableName.ThisVariableName] = new ThisVariableReference(_method);
            }
        }

        LocalVariableReference CreateAutoGlobal(VariableName name, TextSpan span)
        {
            throw new NotImplementedException();
        }

        LocalVariableReference CreateLocal(VariableName name, VariableKind kind, VarDeclarator decl)
        {
            Debug.Assert(!name.IsAutoGlobal);
            var locSym = new SourceLocalSymbol(Method, decl);

            return new LocalVariableReference(kind, Method, locSym, new BoundVariableName(name, locSym.Type));
        }

        #region Public methods

        public bool TryGetVariable(VariableName varname, out LocalVariableReference variable) =>
            _dict.TryGetValue(varname, out variable);

        IVariableReference BindVariable(VariableName varname, TextSpan span,
            Func<VariableName, TextSpan, LocalVariableReference> factory)
        {
            if (!_dict.TryGetValue(varname, out var value))
            {
                _dict[varname] = value = factory(varname, span);
            }

            //
            Debug.Assert(value != null);
            return value;
        }

        /// <summary>
        /// Gets local variable or create local if not yet.
        /// </summary>
        public IVariableReference BindLocalVariable(VariableName varname, VarDeclarator decl) => BindVariable(varname,
            decl.Span.ToTextSpan(), (name, span) => CreateLocal(name, VariableKind.LocalVariable, decl));

        // public IVariableReference BindTemporalVariable(VariableName varname) => BindVariable(varname, default,
        //     (name, span) => CreateLocal(name, VariableKind.LocalTemporalVariable, span));
        //
        // public IVariableReference BindAutoGlobalVariable(VariableName varname) =>
        //     BindVariable(varname, default, (name, span) => CreateAutoGlobal(name, span));

        #endregion
    }
}