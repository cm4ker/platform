using System;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Compiler.Utilities;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Symbols.Source
{
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

    internal class SourceGlobalNamespaceSymbol : NamespaceSymbol
    {
        readonly SourceModuleSymbol _sourceModule;
        private ImmutableArray<MethodSymbol> _exntensionMethods;
        private ImmutableArray<NamedTypeSymbol> _userVisibleTypes;


        public SourceGlobalNamespaceSymbol(SourceModuleSymbol module)
        {
            _sourceModule = module;
        }

        internal override AquilaCompilation DeclaringCompilation => _sourceModule.DeclaringCompilation;

        public override AquilaCompilation ContainingCompilation => _sourceModule.DeclaringCompilation;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespace(this);

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor) => visitor.VisitNamespace(this);

        public override AssemblySymbol ContainingAssembly => _sourceModule.ContainingAssembly;

        internal override ModuleSymbol ContainingModule => _sourceModule;

        public override Symbol ContainingSymbol => _sourceModule;

        public override string Name => string.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        private void EnsureExtensionMethods()
        {
            if (!_exntensionMethods.IsDefault)
                return;

            var builder = new ArrayBuilder<MethodSymbol>();

            var syms = DeclaringCompilation.GetBoundReferenceManager().ExplicitReferencesSymbols;
            foreach (AssemblySymbol sym in syms)
            {
                var gns = sym.Modules[0].GlobalNamespace;

                HandleNs(gns);

                void HandleNs(NamespaceSymbol ns)
                {
                    var members = ns.GetMembers();

                    foreach (var member in members)
                    {
                        if (member.Kind == SymbolKind.Namespace)
                        {
                            var nestedNs = (NamespaceSymbol)member;
                            HandleNs(nestedNs);
                        }

                        if (member.Kind == SymbolKind.NamedType &&
                            member.GetAttribute(CoreTypes.AquilaExtensionAqAttributeFullName) != null)
                        {
                            var ext = (NamedTypeSymbol)member;
                            var methods = ext.GetMembers().OfType<MethodSymbol>()
                                .Where(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public);

                            builder.AddRange(methods);
                        }
                    }
                }
            }

            _exntensionMethods = builder.ToImmutableAndFree();
        }

        private void EnsureUserVisibleTypes()
        {
            if (!_userVisibleTypes.IsDefault)
                return;

            var builder = new ArrayBuilder<NamedTypeSymbol>();

            var syms = DeclaringCompilation.GetBoundReferenceManager().ExplicitReferencesSymbols;
            foreach (AssemblySymbol sym in syms)
            {
                var gns = sym.Modules[0].GlobalNamespace;

                HandleNs(gns);

                void HandleNs(NamespaceSymbol ns)
                {
                    var members = ns.GetMembers();

                    foreach (var member in members)
                    {
                        if (member.Kind == SymbolKind.Namespace)
                        {
                            var nestedNs = (NamespaceSymbol)member;
                            HandleNs(nestedNs);
                        }

                        if (member.Kind == SymbolKind.NamedType &&
                            member.GetAttribute(CoreTypes.AquilaUserVisibleAttributeFullName) != null)
                        {
                            builder.AddRange((NamedTypeSymbol)member);
                        }
                    }
                }
            }

            _userVisibleTypes = builder.ToImmutableAndFree();
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return DeclaringCompilation.PlatformSymbolCollection.GetNamespaces().OfType<Symbol>().ToImmutableArray();
        }


        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            EnsureExtensionMethods();
            EnsureUserVisibleTypes();

            var arr = new ArrayBuilder<Symbol>();

            var ns = DeclaringCompilation.PlatformSymbolCollection.GetNamespace(name);

            if (ns != null)
                arr.Add(ns);

            var result = _exntensionMethods.Where(x => x.Name == name);
            arr.AddRange(result);

            return arr.ToImmutableAndFree();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            EnsureUserVisibleTypes();
            return _sourceModule.DeclaringCompilation.PlatformSymbolCollection.GetAllCreatedTypes().AsImmutable();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            EnsureUserVisibleTypes();

            var x = _sourceModule.SymbolCollection.GetType(NameUtils.MakeQualifiedName(name, true));
            if (x != null)
            {
                if (x.IsErrorType())
                {
                    var candidates = ((ErrorTypeSymbol)x).CandidateSymbols;
                    if (candidates.Length != 0)
                    {
                        return candidates.OfType<NamedTypeSymbol>().AsImmutable();
                    }
                }
                else
                {
                    return ImmutableArray.Create(x);
                }
            }

            var builder = new ArrayBuilder<NamedTypeSymbol>();
            var types = _userVisibleTypes.Where(t => t.Name == name).ToList();

            //force return result
            if (types.Any())
            {
                builder.AddRange(types);
                return builder.ToImmutableArray();
            }

            var type = _sourceModule.DeclaringCompilation.PlatformSymbolCollection
                .GetType(QualifiedName.Parse(name, false));

            if (type != null)
                builder.Add(type);


            return builder.ToImmutableAndFree();
        }
    }
}