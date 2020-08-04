﻿using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// A <see cref="MissingNamespaceSymbol"/> is a special kind of <see cref="NamespaceSymbol"/> that represents
    /// a namespace that couldn't be found.
    /// </summary>
    internal class MissingNamespaceSymbol : NamespaceSymbol
    {
        private readonly string _name;
        private readonly Symbol _containingSymbol;

        //public MissingNamespaceSymbol(MissingModuleSymbol containingModule)
        //{
        //    Debug.Assert((object)containingModule != null);

        //    _containingSymbol = containingModule;
        //    _name = string.Empty;
        //}

        public MissingNamespaceSymbol(NamespaceSymbol containingNamespace, string name)
        {
            Debug.Assert((object)containingNamespace != null);
            Debug.Assert(name != null);

            _containingSymbol = containingNamespace;
            _name = name;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return _containingSymbol;
            }
        }

        public override AssemblySymbol ContainingAssembly
        {
            get
            {
                return _containingSymbol.ContainingAssembly;
            }
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_containingSymbol.GetHashCode(), _name.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            MissingNamespaceSymbol other = obj as MissingNamespaceSymbol;

            return (object)other != null && _name.Equals(other._name) && _containingSymbol.Equals(other._containingSymbol);
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ImmutableArray<Location>.Empty;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }
    }
}
