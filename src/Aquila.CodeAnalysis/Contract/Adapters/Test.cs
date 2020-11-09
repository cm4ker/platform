using Aquila.CodeAnalysis.Public;
using Microsoft.CodeAnalysis;

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
        
        #region Setters

        public void SetIsAbstract(bool value)
        {
            _isAbstract = value;
        }

        public void SetIsStatic(bool value)
        {
            _isStatic = value;
        }

        public void SetName(string value)
        {
            _name = value;
        }

        public void SetNamespace(string value)
        {
            _namespace = value;
        }

        public void SetNamespace(INamedTypeSymbol value)
        {
            _baseType = (NamedTypeSymbol) value;
        }

        #endregion
        
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