using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Compiler.Utilities;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Manages context of local variables, their merged type and return value type.
    /// </summary>
    public class FlowContext
    {
        internal class TypeRefInfo
        {
            private readonly TypeSymbol _symbol;

            /// <summary>
            /// Type was passed by reference
            /// </summary>
            private bool _byReference;

            public TypeRefInfo(TypeSymbol symbol)
            {
                _symbol = symbol;
            }

            public bool ByRef => _byReference;

            internal void SetByRef(bool value) => _byReference = value;
        }

        private int _version;
        private readonly SourceMethodSymbolBase _method;
        private TypeRefInfo[] _varsType = Array.Empty<TypeRefInfo>();
        private List<SourceLambdaSymbol> _lambdas = new List<SourceLambdaSymbol>(); 


        internal FlowContext(SourceMethodSymbolBase method)
        {
            _method = method;
            _varsIndex = new Dictionary<VariableName, int>();
        }

        /// <summary>
        /// Size of ulong bit array (<c>64</c>).
        /// </summary>
        internal const int BitsCount = sizeof(ulong) * 8;

        /// <summary>
        /// Reference to corresponding method symbol. Can be a <c>null</c> reference.
        /// </summary>
        internal SourceMethodSymbolBase Method => _method;

        /// <summary>
        /// Map of variables name and their index.
        /// </summary>
        readonly Dictionary<VariableName, int> _varsIndex;

        /// <summary>
        /// Merged local variables type.
        /// </summary>
        internal TypeRefInfo[] VarsType => _varsType;


        internal ImmutableArray<SourceLambdaSymbol> Lambdas => _lambdas.ToImmutableArray();

        /// <summary>
        /// Version of the analysis, incremented whenever a set of semantic tree transformations happen.
        /// </summary>
        internal int Version => _version;

        internal void AddVarType(int varIndex, TypeSymbol type)
        {
            if (varIndex >= 0 && varIndex < _varsType.Length)
            {
                _varsType[varIndex] = new TypeRefInfo(type);
            }
        }

        internal void AddLambda(SourceLambdaSymbol lambda) 
            => _lambdas.Add(lambda);
        
        /// <summary>
        /// Gets index of variable within the context.
        /// </summary>
        public VariableHandle GetVarIndex(VariableName name)
        {
            Debug.Assert(name.IsValid(), $"Invalid variable name '{name.Value}' in method '{_method.Name}'");

            if (_varsIndex.TryGetValue(name, out var index)) 
                return new VariableHandle() { Slot = index, Name = name };
            
            lock (_varsIndex)
            {
                index = _varsType.Length;
                Array.Resize(ref _varsType, index + 1);

                _varsIndex[name] = index;
            }

            return new VariableHandle() { Slot = index, Name = name };
        }

        public void SetReference(int varIndex)
        {
            if (varIndex >= 0 && varIndex < _varsType.Length)
            {
                _varsType[varIndex].SetByRef(true);
            }
        }

        /// <summary>
        /// Gets value indicating whether given variable might be a reference.
        /// </summary>
        public bool IsReference(int varIndex)
        {
            // anything >= 64 is reported as a possible reference
            return varIndex < 0 || varIndex >= _varsType.Length || _varsType[varIndex].ByRef;
        }
        
        /// <summary>
        /// Discard the current flow analysis information, should be called whenever the method is transformed.
        /// </summary>
        /// <remarks>
        /// It is expected to be called either on a context without a method (parameter initializers etc.) or
        /// on a method with a CFG, hence no abstract methods etc.
        /// </remarks>
        public void InvalidateAnalysis()
        {
            Debug.Assert(Method?.ControlFlowGraph != null);

            // By incrementing the version, the current flow states won't be valid any longer
            _version++;

            // Reset internal structures to prevent possible bugs in re-analysis
            _varsIndex.Clear();
            _varsType = Array.Empty<TypeRefInfo>();
            _lambdas = new List<SourceLambdaSymbol>();

            if (_method == null) return;
            
            // Reset method properties related to the analysis
            _method.IsReturnAnalysed = false;

            // Recreate the entry state to enable another analysis
            _method.ControlFlowGraph.Start.FlowState = StateBinder.CreateInitialState(_method, this);
        }
    }
}