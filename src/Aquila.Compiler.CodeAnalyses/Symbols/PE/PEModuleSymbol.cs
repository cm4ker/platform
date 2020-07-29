using System.Diagnostics;
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

        public override AssemblySymbol ContainingAssembly => _assemblySymbol;
        public override Symbol ContainingSymbol => _assemblySymbol;
        public IModule Module => _module;

        public override NamespaceSymbol GlobalNamespace => _globalNamespace;

        internal override NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName)
        {
            _globalNamespace.LoadAllTypes();

            NamedTypeSymbol result;
            NamespaceSymbol scope = this.GlobalNamespace.LookupNestedNamespace(emittedName.NamespaceSegments);

            if ((object) scope == null)
            {
                // We failed to locate the namespace
                //result = new MissingMetadataTypeSymbol.TopLevel(this, ref emittedName);
                result = null;
            }
            else
            {
                result = scope.LookupMetadataType(ref emittedName);
            }

            return result;
        }
    }
}