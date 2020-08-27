using System.Collections.Generic;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Generators
{
    /// <summary>
    /// Represents the api for injectings structs
    /// </summary>
    public interface IGenerator
    {
        void Init(PhpCompilation compilation);

        public IEnumerable<INamedTypeSymbol> Tests();
    }
}