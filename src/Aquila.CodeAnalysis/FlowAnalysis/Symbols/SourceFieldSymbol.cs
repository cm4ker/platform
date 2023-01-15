using Aquila.CodeAnalysis.FlowAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceFieldSymbol
    {
        /// <summary>
        /// Field's <see cref="TypeRefContext"/> instance.
        /// </summary>
        internal TypeRefContext EnsureTypeRefContext() => _typeCtx;

        TypeRefContext _typeCtx;
    }
}
