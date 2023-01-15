using System.Collections.Generic;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Emit
{
    internal abstract class TypeMemberReference : Cci.ITypeMemberReference
    {
        protected abstract Aquila.CodeAnalysis.Symbols.Symbol UnderlyingSymbol { get; }

        public virtual Cci.ITypeReference GetContainingType(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return moduleBeingBuilt.Translate(UnderlyingSymbol.ContainingType, context.SyntaxNode, context.Diagnostics);
        }

        string Cci.INamedEntity.Name
        {
            get { return UnderlyingSymbol.MetadataName; }
        }

        IEnumerable<Cci.ICustomAttribute> Cci.IReference.GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<Cci.ICustomAttribute>();
        }

        public abstract void Dispatch(Cci.MetadataVisitor visitor);

        Cci.IDefinition Cci.IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        public ISymbolInternal? GetInternalSymbol()
        {
            throw new System.NotImplementedException();
        }
    }
}