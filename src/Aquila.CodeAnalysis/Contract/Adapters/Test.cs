using Aquila.CodeAnalysis.Public;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class NamedTypeSymbol : IType
    {
    }

    partial class MethodSymbol : IMethod
    {
    }
}

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    partial class SynthesizedTypeSymbol : ITypeBuilder
    {
        public IMethodBuilder CreateMethod()
        {
            var method = new SynthesizedMethodSymbol(this);
            _lazyMembers.Add(method);

            return method;
        }
    }

    partial class SynthesizedMethodSymbol : IMethodBuilder
    {
    }
}