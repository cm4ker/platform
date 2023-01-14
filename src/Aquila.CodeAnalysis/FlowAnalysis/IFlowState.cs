using System;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Represents a variable in the method context.
    /// </summary>
    [DebuggerDisplay("${Name.Value,nq}#{Slot}")]
    public struct VariableHandle : IEquatable<VariableHandle>
    {
        /// <summary>
        /// Valid indexes starts from <c>1</c>.
        /// </summary>
        int _index;

        /// <summary>
        /// The variable name.
        /// </summary>
        VariableName _name;

        /// <summary>
        /// Gets value indicating the handle is valid.
        /// </summary>
        public bool IsValid => (_index > 0);

        /// <summary>
        /// throws an exception if the handle is not valid.
        /// </summary>
        internal void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets or sets internal slot within the locals variable table starting from <c>0</c>.
        /// </summary>
        public int Slot
        {
            get { return _index - 1; }
            set { _index = value + 1; }
        }

        /// <summary>
        /// The variable name.
        /// </summary>
        public VariableName Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        #region IEquatable<VariableHandle>

        bool IEquatable<VariableHandle>.Equals(VariableHandle other)
        {
            return _index == other._index;
        }

        public override int GetHashCode() => _index * 2;

        public override bool Equals(object obj) => obj is VariableHandle && ((VariableHandle) obj)._index == _index;

        #endregion

        /// <summary>
        /// Implicitly converts the handle to an integer slot index.
        /// </summary>
        public static implicit operator int(VariableHandle handle) => handle.Slot;
    }
}