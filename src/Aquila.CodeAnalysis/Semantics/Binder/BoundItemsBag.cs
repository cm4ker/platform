using System;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics.Graph;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Holds currently bound item and optionally the first and the last BoundBlock containing all the statements that are supposed to go before the BoundElement. 
    /// </summary>
    /// <typeparam name="T">Either <c>BoundExpression</c> or <c>BoundStatement</c>.</typeparam>
    public struct BoundItemsBag<T> : IEquatable<BoundItemsBag<T>> where T : class, IAquilaOperation
    {
        public object PreBoundBlockFirst { get; private set; }
        public object PreBoundBlockLast { get; private set; }

        public T BoundElement { get; private set; }

        public BoundItemsBag(T bound, object preBoundFirst = null, object preBoundLast = null)
        {
            Debug.Assert(bound != null || (preBoundFirst == null && preBoundLast == null));
            Debug.Assert(preBoundFirst != null || preBoundLast == null);

            PreBoundBlockFirst = preBoundFirst;
            PreBoundBlockLast = preBoundLast ?? preBoundFirst;
            BoundElement = bound;
        }

        /// <summary>
        /// An empty bag with no item and no pre-bound blocks.
        /// </summary>
        public static BoundItemsBag<T> Empty => new BoundItemsBag<T>(null);

        /// <summary>
        /// Returns bound element and asserts that there are no <c>PreBoundStatements</c>.
        /// </summary>
        public T SingleBoundElement()
        {
            if (!IsOnlyBoundElement)
            {
                throw new InvalidOperationException();
            }

            return BoundElement;
        }

        public static implicit operator BoundItemsBag<T>(T item) => new BoundItemsBag<T>(item);

        public bool IsEmpty => IsOnlyBoundElement && BoundElement == null;

        public bool IsOnlyBoundElement => PreBoundBlockFirst == null;

        public bool Equals(BoundItemsBag<T> b) =>
            BoundElement == b.BoundElement &&
            PreBoundBlockFirst == b.PreBoundBlockFirst &&
            PreBoundBlockLast == b.PreBoundBlockLast;

        public override int GetHashCode() => BoundElement != null ? BoundElement.GetHashCode() : -1;

        public override bool Equals(object obj) => obj is BoundItemsBag<T> bag && Equals(bag);

        public static bool operator ==(BoundItemsBag<T> a, BoundItemsBag<T> b) => a.Equals(b);

        public static bool operator !=(BoundItemsBag<T> a, BoundItemsBag<T> b) => !a.Equals(b);
    }
}