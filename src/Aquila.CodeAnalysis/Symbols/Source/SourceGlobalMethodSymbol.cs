using System.Collections.Generic;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Specifying source global symbol
    /// </summary>
    sealed partial class SourceGlobalMethodSymbol : SourceMethodSymbol
    {
        public SourceGlobalMethodSymbol(NamedTypeSymbol type, FuncDecl syntax) : base(type, syntax)
        {
        }

        public override bool IsGlobalScope => true;

        protected override IEnumerable<ParameterSymbol> BuildImplicitParams()
        {
            int index = 0;

            foreach (var param in base.BuildImplicitParams())
            {
                yield return param;
            }
        }

        public override Accessibility DeclaredAccessibility => Accessibility.Public;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsStatic => true;

        public override bool IsVirtual => false;

        public override bool IsOverride => false;
    }
}