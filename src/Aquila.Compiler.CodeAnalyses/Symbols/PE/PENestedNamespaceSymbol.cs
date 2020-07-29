using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PENestedNamespaceSymbol : PENamespaceSymbol
    {
        private readonly string _name;
        private readonly PENamespaceSymbol _containingNamespace;

        public PENestedNamespaceSymbol(string name, PENamespaceSymbol containingNamespace)
        {
            _name = name;
            _containingNamespace = containingNamespace;
        }

        public override string Name => _name;

        public override IEnumerable<Symbol> GetMembers()
        {
            throw new NotImplementedException();
            // foreach (var type in Types.Select(x => x.Value))
            // {
            //     yield return type;
            // }
            //
            // var nestedMembers = Nested.Select(x => x.Value).SelectMany(x => x.GetMembers());
            //
            // foreach (var nestedType in nestedMembers)
            // {
            //     yield return nestedType;
            // }
        }

        internal override PEModuleSymbol ContainingPEModule => _containingNamespace.ContainingPEModule;
    }
}