using System;
using System.Collections.Generic;
using System.Text;
using Pchp.CodeAnalysis;
using Pchp.CodeAnalysis.FlowAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceFieldSymbol
    {
        /// <summary>
        /// Field's <see cref="TypeRefContext"/> instance.
        /// </summary>
        internal TypeRefContext EnsureTypeRefContext() => _typeCtx; // ?? (_typeCtx = TypeRefFactory.CreateTypeRefContext(_containingType));

        TypeRefContext _typeCtx;
    }
}
