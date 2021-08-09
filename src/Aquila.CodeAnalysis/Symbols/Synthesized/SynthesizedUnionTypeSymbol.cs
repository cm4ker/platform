using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Aquila.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;
using TypeLayout = Microsoft.CodeAnalysis.TypeLayout;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// Describes set of union types
    /// </summary>
    [DebuggerDisplay("Variable: ${Name,nq}")]
    internal sealed class SynthesizedUnionTypeSymbol : SynthesizedTypeSymbol
    {
        private readonly NamespaceOrTypeSymbol _container;
        private readonly ImmutableArray<TypeSymbol> _types;

        public SynthesizedUnionTypeSymbol(NamespaceOrTypeSymbol container, IEnumerable<TypeSymbol> types) : base(
            container, container.DeclaringCompilation)
        {
            _container = container;
            _types = types.ToImmutableArray();
        }

        public ImmutableArray<TypeSymbol> ContainingTypes => _types;

        public override string Name => _types.Aggregate("", (s, symbol) => $"{s}|{symbol.Name}", s => s.TrimStart('|'));

        public override bool IsStatic => false;

        //public override TypeKind TypeKind => TypeKind.Dynamic;


        private ConcurrentBag<Symbol> _lazyMembers;


        public override ImmutableArray<Symbol> GetMembers()
        {
            EnsureMembers();
            return _lazyMembers.ToImmutableArray();
        }

        private void EnsureMembers()
        {
            if (_lazyMembers != null)
                return;

            var typeNum = 0;

            var builder = new ConcurrentBag<Symbol>();

            var ctor = new SynthesizedCtorSymbol(this)
                .SetMethodBuilder((m, db) => { return il => { il.EmitRet(true); }; });
            ;
            var thisArg = new ThisArgPlace(this);

            foreach (var type in _types)
            {
                var field = new SynthesizedFieldSymbol(this);
                field.SetType(type)
                    .SetName($"Container{typeNum}")
                    .SetAccess(Accessibility.Private)
                    .SetIsStatic(false);
                var flStore = new FieldPlace(field);

                var convertMethod = new SynthesizedMethodSymbol(this);
                var param = new SynthesizedParameterSymbol(convertMethod, type, 0, RefKind.None);
                var paramPlace = new ParamPlace(param);

                convertMethod.SetName($"op_Implicit")
                    .SetAccess(Accessibility.Public)
                    .SetIsStatic(true)
                    // .SetVirtual(false)
                    // .SetAbstract(false)
                    .SetParameters(param)
                    .SetReturn(this)
                    .SetMethodBuilder((m, db) =>
                    {
                        return il =>
                        {
                            il.EmitCall(m, db, ILOpCode.Newobj, ctor);
                            il.EmitOpCode(ILOpCode.Dup);
                            paramPlace.EmitLoad(il);
                            flStore.EmitStore(il);
                            il.EmitRet(false);
                        };
                    });


                typeNum++;
                builder.Add(field);
                builder.Add(convertMethod);
            }

            builder.Add(ctor);
            Interlocked.CompareExchange(ref _lazyMembers, builder, null);
        }
    }
}