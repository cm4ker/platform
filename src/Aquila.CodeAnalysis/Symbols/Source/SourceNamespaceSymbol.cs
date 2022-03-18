using System;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    //TODO: remove this or use
    internal partial class SourceNamespaceSymbol : NamespaceSymbol
    {
        readonly SourceModuleSymbol _sourceModule;
        readonly string _name;

        // public SourceNamespaceSymbol(SourceModuleSymbol module, NamespaceDecl ns)
        // {
        //     _sourceModule = module;
        //     _name = ns.QualifiedName.QualifiedName.ClrName();
        // }

        internal override AquilaCompilation DeclaringCompilation => _sourceModule.DeclaringCompilation;

        public override AquilaCompilation ContainingCompilation => _sourceModule.DeclaringCompilation;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespace(this);

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor) => visitor.VisitNamespace(this);

        public override AssemblySymbol ContainingAssembly => _sourceModule.ContainingAssembly;

        internal override ModuleSymbol ContainingModule => _sourceModule;

        public override Symbol ContainingSymbol => _sourceModule;

        public override string Name => _name;

        internal override NamespaceExtent Extent
        {
            get { return new NamespaceExtent(_sourceModule); }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            throw new NotImplementedException();
        }


        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            throw new NotImplementedException();
        }
    }
}