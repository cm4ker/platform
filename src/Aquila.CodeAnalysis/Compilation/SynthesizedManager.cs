using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Cci = Microsoft.Cci;


namespace Aquila.CodeAnalysis.Emit
{
    /// <summary>
    /// Manages synthesized symbols in the module builder.
    /// </summary>
    internal class SynthesizedManager
    {
        readonly PEModuleBuilder _module;

        public AquilaCompilation DeclaringCompilation => _module.Compilation;

        readonly ConcurrentDictionary<Cci.ITypeDefinition, List<Aquila.CodeAnalysis.Symbols.Symbol>> _membersByType =
            new ConcurrentDictionary<Cci.ITypeDefinition, List<Aquila.CodeAnalysis.Symbols.Symbol>>();

        public SynthesizedManager(PEModuleBuilder module)
        {
            Contract.ThrowIfNull(module);

            _module = module;
        }

        #region Synthesized Members

        List<Aquila.CodeAnalysis.Symbols.Symbol> EnsureList(Cci.ITypeDefinition type)
        {
            return _membersByType.GetOrAdd(type, (_) => new List<Aquila.CodeAnalysis.Symbols.Symbol>());
        }

        void AddMember(Cci.ITypeDefinition type, Aquila.CodeAnalysis.Symbols.Symbol member)
        {
            var members = EnsureList(type);
            lock (members)
            {
                if (members.IndexOf(member) < 0)
                {
                    members.Add(member);
                }
                else
                {
                    Debug.Fail("Member added twice!");
                }
            }
        }

        /// <summary>
        /// Gets or initializes static constructor symbol.
        /// </summary>
        public MethodSymbol EnsureStaticCtor(Cci.ITypeDefinition container)
        {
            Contract.ThrowIfNull(container);

            //if (container is NamedTypeSymbol)
            //{
            //    var cctors = ((NamedTypeSymbol)container).StaticConstructors;
            //    if (!cctors.IsDefaultOrEmpty)
            //    {
            //        return cctors[0];
            //    }
            //}

            //
            var members = EnsureList(container);
            lock (members)
            {
                //
                var cctor = members.OfType<SynthesizedCctorSymbol>().FirstOrDefault();
                if (cctor == null)
                {
                    // cctor = new SynthesizedCctorSymbol(container, DeclaringCompilation.SourceModule);
                    // members.Add(cctor);
                }

                return cctor;
            }

            //
        }

        /// <summary>
        /// Creates synthesized field.
        /// </summary>
        public SynthesizedFieldSymbol GetOrCreateSynthesizedField(NamedTypeSymbol container, TypeSymbol type,
            string name, Accessibility accessibility, bool isstatic, bool @readonly, bool autoincrement = false)
        {
            SynthesizedFieldSymbol field = null;

            var members = EnsureList(container);
            lock (members)
            {
                if (autoincrement)
                {
                    name += "?" + members.Count.ToString("x");
                }

                if (!autoincrement)
                {
                    field = members
                        .OfType<SynthesizedFieldSymbol>()
                        .FirstOrDefault(f =>
                            f.Name == name && f.IsStatic == isstatic && f.Type == type && f.IsReadOnly == @readonly);
                }

                if (field == null)
                {
                    field = new SynthesizedFieldSymbol(container)
                        .SetType(type)
                        .SetName(name)
                        .SetAccess(accessibility)
                        .SetIsStatic(isstatic)
                        .SetReadOnly(@readonly);;
                    members.Add(field);
                }
            }

            Debug.Assert(field.IsImplicitlyDeclared);

            return field;
        }

        /// <summary>
        /// Adds a type member to the class.
        /// </summary>
        /// <param name="container">Containing type.</param>
        /// <param name="nestedType">Type to be added as nested type.</param>
        public void AddNestedType(Cci.ITypeDefinition container, NamedTypeSymbol nestedType)
        {
            Contract.ThrowIfNull(nestedType);
            Debug.Assert(nestedType.IsImplicitlyDeclared);
            Debug.Assert((container as ISymbol)?.ContainingType == null); // can't nest in nested type

            AddMember(container, nestedType);
        }

        /// <summary>
        /// Adds a synthesized method to the class.
        /// </summary>
        public void AddMethod(Cci.ITypeDefinition container, MethodSymbol method)
        {
            Contract.ThrowIfNull(method);
            Debug.Assert(method.IsImplicitlyDeclared);

            AddMember(container, method);
        }

        /// <summary>
        /// Adds a synthesized property to the class.
        /// </summary>
        public void AddProperty(Cci.ITypeDefinition container, PropertySymbol property)
        {
            Contract.ThrowIfNull(property);

            AddMember(container, property);
        }

        /// <summary>
        /// Adds a synthesized symbol to the class.
        /// </summary>
        public void AddField(Cci.ITypeDefinition container, FieldSymbol field)
        {
            AddMember(container, field);
        }

        /// <summary>
        /// Gets synthezised members contained in <paramref name="container"/>.
        /// </summary>
        /// <remarks>
        /// This method is not thread-safe, it is expected to be called after all
        /// the synthesized members were added to <paramref name="container"/>.
        /// </remarks>
        /// <typeparam name="T">Type of members to enumerate.</typeparam>
        /// <param name="container">Containing type.</param>
        /// <returns>Enumeration of synthesized type members.</returns>
        public IEnumerable<T> GetMembers<T>(Cci.ITypeDefinition container) where T : ISymbol
        {
            List<Aquila.CodeAnalysis.Symbols.Symbol> list;
            if (_membersByType.TryGetValue(container, out list) && list.Count != 0)
            {
                return list.OfType<T>();
            }
            else
            {
                return SpecializedCollections.EmptyEnumerable<T>();
            }
        }

        #endregion
    }
}