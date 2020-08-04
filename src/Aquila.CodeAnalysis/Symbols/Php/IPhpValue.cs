using Pchp.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.Symbols.Php
{
    /// <summary>
    /// An interface of symbols with a result value (field, routine, property).
    /// </summary>
    public interface IPhpValue
    {
        /// <summary>
        /// Optional. Gets the initializer.
        /// </summary>
        BoundExpression Initializer { get; }

        /// <summary>
        /// Gets flag indicating the value cannot be <c>NULL</c>.
        /// </summary>
        bool HasNotNull { get; }
    }
}
