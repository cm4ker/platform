using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        #region Constants

        /// <summary>
        /// Size of ulong bit array (<c>64</c>).
        /// </summary>
        internal const int BitsCount = sizeof(ulong) * 8;

        #endregion

        #region Fields & Properties

        /// <summary>
        /// Associated type context.
        /// </summary>
        public TypeRefContext TypeRefContext => _typeCtx;

        readonly TypeRefContext
            _typeCtx;

        /// <summary>
        /// Reference to corresponding method symbol. Can be a <c>null</c> reference.
        /// </summary>
        internal SourceMethodSymbolBase Method => _method;

        readonly SourceMethodSymbolBase _method;

        /// <summary>
        /// Map of variables name and their index.
        /// </summary>
        readonly Dictionary<VariableName, int>
            _varsIndex;

        /// <summary>
        /// Bit mask of variables where bit with value <c>1</c> signalizes that variables with index corresponding to the bit number has been used.
        /// </summary>
        ulong _usedMask;

        /// <summary>
        /// Merged local variables type.
        /// </summary>
        internal TypeRefInfo[] VarsType => _varsType;

        TypeRefInfo[]
            _varsType = Array.Empty<TypeRefInfo>();

        /// <summary>
        /// Merged return expressions type.
        /// </summary>
        internal TypeRefInfo ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

        TypeRefInfo _returnType;

        /// <summary>
        /// Version of the analysis, incremented whenever a set of semantic tree transformations happen.
        /// </summary>
        internal int Version => _version;

        int _version;

        #endregion

        #region Construction

        internal FlowContext(SourceMethodSymbolBase method)
        {
            _method = method;

            //
            _varsIndex = new Dictionary<VariableName, int>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets index of variable within the context.
        /// </summary>
        public VariableHandle GetVarIndex(VariableName name)
        {
            Debug.Assert(name.IsValid(), $"Invalid variable name '{name.Value}' in method '{_method.Name}'");

            // TODO: RW lock

            int index;
            if (!_varsIndex.TryGetValue(name, out index))
            {
                lock (_varsIndex)
                {
                    index = _varsType.Length;
                    Array.Resize(ref _varsType, index + 1);

                    //
                    _varsIndex[name] = index;
                }
            }

            //
            return new VariableHandle() { Slot = index, Name = name };
        }

        /// <summary>
        /// Enumerates all known variables as pairs of their index and name.
        /// </summary>
        public IEnumerable<VariableHandle> EnumerateVariables()
        {
            return _varsIndex.Select(pair => new VariableHandle()
            {
                Slot = pair.Value,
                Name = pair.Key,
            });
        }

        public void SetReference(int varindex)
        {
            if (varindex >= 0 && varindex < _varsType.Length)
            {
                _varsType[varindex].SetByRef(true);
            }
        }

        /// <summary>
        /// Gets value indicating whether given variable might be a reference.
        /// </summary>
        public bool IsReference(int varindex)
        {
            // anything >= 64 is reported as a possible reference
            return varindex < 0 || varindex >= _varsType.Length || _varsType[varindex].ByRef;
        }

        internal void AddVarType(int varindex, TypeSymbol type)
        {
            if (varindex >= 0 && varindex < _varsType.Length)
            {
                _varsType[varindex] = new TypeRefInfo(type);
            }
        }

        internal TypeRefInfo GetVarType(VariableName name)
        {
            var idx = GetVarIndex(name);
            return _varsType[idx];
        }

        /// <summary>
        /// Sets specified variable as being used.
        /// </summary>
        public void SetUsed(int varindex)
        {
            if (varindex >= 0 && varindex < BitsCount)
            {
                _usedMask |= (ulong)1 << varindex;
            }
        }

        /// <summary>
        /// Marks all local variables as used.
        /// </summary>
        public void SetAllUsed()
        {
            _usedMask = ~(ulong)0;
        }

        public bool IsUsed(int varindex)
        {
            // anything >= 64 is used
            return varindex < 0 || varindex >= BitsCount || (_usedMask & (ulong)1 << varindex) != 0;
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
            _usedMask = 0;
            _varsIndex.Clear();
            _varsType = Array.Empty<TypeRefInfo>();

            // Revert the information regarding the return type to the default state
            ReturnType = default;

            // TODO: Recreate the state also in the case of a standalone expression (such as a parameter initializer)
            if (_method != null)
            {
                // Reset method properties related to the analysis
                _method.IsReturnAnalysed = false;

                // Recreate the entry state to enable another analysis
                _method.ControlFlowGraph.Start.FlowState = StateBinder.CreateInitialState(_method, this);
            }
        }

        #endregion
    }
}