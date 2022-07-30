using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Utilities;


namespace Aquila.CodeAnalysis.FlowAnalysis
{
    public sealed class TypeRefContext
    {
        private readonly AquilaCompilation _compilation;
        private readonly TypeSymbol _thisType;
        private readonly TypeSymbol _type;

        internal TypeRefContext(AquilaCompilation compilation, TypeSymbol thisType, TypeSymbol type)
        {
            _compilation = compilation;
            _thisType = thisType;
            _type = type;
        }
    }
}