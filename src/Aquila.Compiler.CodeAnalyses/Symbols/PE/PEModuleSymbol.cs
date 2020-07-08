using Aquila.Compiler.Contracts;
using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols.PE
{
    public class PEModuleSymbol : ModuleSymbol
    {
        private readonly AssemblySymbol _assemblySymbol;
        private readonly IModule _module;
        private PEGlobalNamespaceSymbol _globalNamespace;

        public PEModuleSymbol(AssemblySymbol assemblySymbol, IModule module)
        {
            _assemblySymbol = assemblySymbol;
            _module = module;
            _globalNamespace = new PEGlobalNamespaceSymbol(this);
        }

        public AssemblySymbol ContainingAssembly => _assemblySymbol;

        public override Symbol ContainingSymbol => _assemblySymbol;
        public IModule Module => _module;

        internal override NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName)
        {
            throw new System.NotImplementedException();
        }
    }
}