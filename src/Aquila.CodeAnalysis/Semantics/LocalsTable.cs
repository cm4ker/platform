using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Table of local variables used within method.
    /// </summary>
    internal sealed class LocalsTable
    {
        #region Fields & Properties

        readonly Dictionary<VariableName, LocalVariableReference> _dict = new();

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
        public SourceMethodSymbolBase Method => _method;

        readonly SourceMethodSymbolBase _method;

        #endregion

        /// <summary>
        /// Initializes table of locals of given method.
        /// </summary>
        public LocalsTable(SourceMethodSymbolBase method)
        {
            Contract.ThrowIfNull(method);

            _method = method;

            PopulateParameters();
        }

        private void PopulateParameters()
        {
            foreach (var p in _method.SourceParameters)
            {
                _dict[new VariableName(p.Name)] = new ParameterReference(p, Method);
            }

            if (!_method.IsStatic)
            {
                _dict[VariableName.ThisVariableName] = new ThisVariableReference(_method);
            }
        }

        LocalVariableReference CreateLocal(VariableName name, VariableKind kind, VariableInit decl)
        {
            var locSym = new SourceLocalSymbol(Method, decl);

            Method.Flags |= MethodFlags.UsesLocals;

            return new LocalVariableReference(kind, Method, locSym, new BoundVariableName(name, locSym.Type));
        }

        LocalVariableReference CreateLocal(VariableName name, VariableKind kind, TypeSymbol type)
        {
            var locSym = new SynthesizedLocalSymbol(Method, name.Value, type);

            Method.Flags |= MethodFlags.UsesLocals;

            return new LocalVariableReference(kind, Method, locSym, new BoundVariableName(name, locSym.Type));
        }


        #region Public methods

        public bool TryGetVariable(string name, out LocalVariableReference variable) =>
            _dict.TryGetValue(new VariableName(name), out variable);

        public bool TryGetVariable(VariableName varname, out LocalVariableReference variable) =>
            _dict.TryGetValue(varname, out variable);

        IVariableReference BindVariable(VariableName varname,
            Func<VariableName, LocalVariableReference> factory)
        {
            if (!_dict.TryGetValue(varname, out var value))
            {
                _dict[varname] = value = factory(varname);
            }

            //
            Debug.Assert(value != null);
            return value;
        }

        /// <summary>
        /// Gets local variable or create local if not yet.
        /// </summary>
        public IVariableReference BindLocalVariable(VariableName varname, VariableInit decl) => BindVariable(varname,
            name => CreateLocal(name, VariableKind.LocalVariable, decl));

        public IVariableReference BindTemporalVariable(VariableName varname, TypeSymbol type) =>
            BindVariable(varname, name => CreateLocal(name, VariableKind.LocalTemporalVariable, type));

        public IVariableReference BindLocalVariable(VariableName varName, TextSpan span, TypeSymbol type) =>
            BindVariable(varName, name => new LocalVariableReference(VariableKind.LocalVariable, _method,
                new InPlaceSourceLocalSymbol(_method, name, span, type), new BoundVariableName(name, type)));

        #endregion
    }
}