using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.Compiler.Contracts;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PEAssemblySymbol : MetadataAssemblySymbol
    {
        private readonly IAssembly _asm;
        private ImmutableArray<PEModuleSymbol> _modules = ImmutableArray<PEModuleSymbol>.Empty;

        public PEAssemblySymbol(IAssembly asm)
        {
            _asm = asm;

            foreach (var module in _asm.Modules)
            {
                _modules = _modules.Add(new PEModuleSymbol(this, module));
            }

        }

        
        
        internal IAssembly Assembly => _asm;

        public override ImmutableArray<ModuleSymbol> Modules => _modules.OfType<ModuleSymbol>().ToImmutableArray();
    }
}