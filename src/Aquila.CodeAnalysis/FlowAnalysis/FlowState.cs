using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    internal class FlowState : IEquatable<FlowState>
    {
        #region Fields & Properties

        /// <summary>
        /// Gets flow context.
        /// </summary>
        public FlowContext FlowContext { get; }

        /// <summary>
        /// Source method.
        /// Can be <c>null</c>.
        /// </summary>
        public SourceMethodSymbolBase Method => FlowContext.Method;

        /// <summary>
        /// Types of variables in this state.
        /// </summary>
        private TypeSymbol[] _varsType = Array.Empty<TypeSymbol>();

        /// <summary>
        /// Mask of initialized variables in this state.
        /// </summary>
        /// <remarks>
        /// Single bits indicates the corresponding variable was set.
        /// <c>0</c> determines the variable was not set in any code path.
        /// <c>1</c> determines the variable may be set.
        /// </remarks>
        ulong _initializedMask;

        /// <summary>
        /// Version of the analysis this state was created for.
        /// </summary>
        internal int Version { get; }

        #endregion

        #region Construction & Copying

        /// <summary>
        /// Merge constructor.
        /// </summary>
        public FlowState(FlowState state1, FlowState state2)
        {
            Debug.Assert(state1 != null);
            Debug.Assert(state2 != null);
            Debug.Assert(state1.FlowContext == state2.FlowContext);
            Debug.Assert(state1.Version == state2.Version);

            FlowContext = state1.FlowContext;
            _initializedMask = state1._initializedMask | state2._initializedMask;

            // intersection of other variable flags
            if (state1._notes != null && state2._notes != null)
            {
                _notes = new HashSet<NoteData>(state1._notes);
                _notes.Intersect(state2._notes);
            }

            Version = state1.Version;
        }

        /// <summary>
        /// Initial locals state for the Start block.
        /// </summary>
        internal FlowState(FlowContext flowCtx)
        {
            Contract.ThrowIfNull(flowCtx);

            FlowContext = flowCtx;
            _initializedMask = (ulong) 0;

            // initial size of the array
            var countHint = (flowCtx.Method != null)
                ? flowCtx.VarsInfo.Length
                : 0;
            _varsType = new TypeSymbol[countHint];

            Version = flowCtx.Version;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public FlowState(FlowState other)
            : this(other.FlowContext, other._varsType)
        {
            // clone internal state

            _initializedMask = other._initializedMask;

            if (other._notes != null)
            {
                _notes = new HashSet<NoteData>(other._notes);
            }

            Version = other.Version;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        private FlowState(FlowContext flowCtx, TypeSymbol[] varsType)
        {
            Contract.ThrowIfNull(flowCtx);
            Contract.ThrowIfNull(varsType);

            FlowContext = flowCtx;
            _varsType = (TypeSymbol[]) varsType.Clone();
            Version = flowCtx.Version;
        }

        #endregion

        #region IEquatable<FlowState> Members

        public bool Equals(FlowState other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other == null ||
                other.FlowContext != FlowContext ||
                other._initializedMask != _initializedMask)
                return false;

            return EnumeratorExtension.EqualEntries(_varsType, other._varsType);
        }

        public override int GetHashCode()
        {
            var hash = this.FlowContext.GetHashCode();
            foreach (var t in _varsType)
            {
                hash ^= t.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FlowState);
        }

        #endregion

        #region IFlowState<FlowState>

        /// <summary>
        /// Creates copy of this state.
        /// </summary>
        public FlowState Clone() => new FlowState(this);

        /// <summary>
        /// Creates new state as a merge of this one and the other.
        /// </summary>
        public FlowState Merge(FlowState other) => new FlowState(this, other);

        /// <summary>
        /// Gets variable handle use for other variable operations.
        /// </summary>
        public VariableHandle GetLocalHandle(VariableName varname)
        {
            return FlowContext.GetVarIndex(varname);
        }

        public TypeSymbol GetLocalType(VariableHandle handle)
        {
            return _varsType[handle];
        }

        /// <summary>
        /// Sets variable type in this state.
        /// </summary>
        /// <param name="handle">Variable handle.</param>
        public void SetLocalType(VariableHandle handle, TypeSymbol type)
        {
            handle.ThrowIfInvalid();

            if (handle >= _varsType.Length)
            {
                Array.Resize(ref _varsType, handle + 1);
            }

            _varsType[handle] = type;

            this.FlowContext.AddVarType(handle, type);

            // update the _initializedMask
            SetVarInitialized(handle);
        }

        /// <summary>
        /// Handles use of a local variable.
        /// </summary>
        public static void VisitLocal(VariableHandle handle)
        {
            handle.ThrowIfInvalid();
        }

        public void SetVarInitialized(VariableHandle handle)
        {
            int varindex = handle.Slot;
            if (varindex >= 0 && varindex < FlowContext.BitsCount)
            {
                _initializedMask |= 1ul << varindex;
            }
        }

        public void SetVarUninitialized(VariableHandle handle)
        {
            var varindex = handle.Slot;
            if (varindex >= 0 && varindex < FlowContext.BitsCount)
            {
                _initializedMask &= ~(1ul << varindex);
            }
        }

        #endregion

        #region Constraints (Notes about variables)

        enum NoteKind
        {
            /// <summary>
            /// Noting that variable is less than Long.Max.
            /// </summary>
            LessThanLongMax,

            /// <summary>
            /// Noting that variable is greater than Long.Min.
            /// </summary>
            GreaterThanLongMin,
        }

        struct NoteData : IEquatable<NoteData>
        {
            public VariableHandle Variable;
            public NoteKind Kind;

            public NoteData(VariableHandle variable, NoteKind kind)
            {
                this.Variable = variable;
                this.Kind = kind;
            }

            public override int GetHashCode() => Variable.GetHashCode() ^ (int) Kind;

            public bool Equals(NoteData other) => this.Variable == other.Variable && this.Kind == other.Kind;
        }

        HashSet<NoteData> _notes;

        bool HasConstrain(VariableHandle variable, NoteKind kind) =>
            _notes != null && _notes.Contains(new NoteData(variable, kind));

        void SetConstrain(VariableHandle variable, NoteKind kind, bool set)
        {
            if (set) AddConstrain(variable, kind);
            else RemoveConstrain(variable, kind);
        }

        void AddConstrain(VariableHandle variable, NoteKind kind)
        {
            if (variable.IsValid)
            {
                var notes = _notes;
                if (notes == null)
                {
                    _notes = notes = new HashSet<NoteData>();
                }

                notes.Add(new NoteData(variable, kind));
            }
        }

        void RemoveConstrain(VariableHandle variable, NoteKind kind)
        {
            if (variable.IsValid)
            {
                var notes = _notes;
                if (notes != null)
                {
                    notes.Remove(new NoteData(variable, kind));

                    if (notes.Count == 0)
                    {
                        _notes = null;
                    }
                }
            }
        }

        /// <summary>
        /// Sets or removes LTInt64 flag for a variable.
        /// </summary>
        public void SetLessThanLongMax(VariableHandle handle, bool lt) =>
            SetConstrain(handle, NoteKind.LessThanLongMax, lt);

        /// <summary>
        /// Gets LTInt64 flag for a variable.
        /// </summary>
        public bool IsLessThanLongMax(VariableHandle handle) => HasConstrain(handle, NoteKind.LessThanLongMax);

        /// <summary>
        /// Sets or removes GTInt64 flag for a variable.
        /// </summary>
        public void SetGreaterThanLongMin(VariableHandle handle, bool lt) =>
            SetConstrain(handle, NoteKind.GreaterThanLongMin, lt);

        /// <summary>
        /// Gets GTInt64 flag for a variable.
        /// </summary>
        public bool IsGreaterThanLongMin(VariableHandle handle) => HasConstrain(handle, NoteKind.GreaterThanLongMin);

        #endregion
    }
}