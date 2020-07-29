using System;
using Aquila.Compiler.Contracts;
using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols.Source
{
    public class SourceAssemblySymbol : AssemblySymbol
    {
        internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
        {
            throw new NotImplementedException();
        }

        public IAssemblyBuilder Builder { get; set; }
    }
}
