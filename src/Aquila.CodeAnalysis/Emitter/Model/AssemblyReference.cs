using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Emitter
{
    internal sealed class AssemblyReference : Cci.IAssemblyReference
    {
        // assembly symbol that represents the target assembly:
        private readonly AssemblySymbol _targetAssembly;

        internal AssemblyReference(AssemblySymbol assemblySymbol)
        {
            Debug.Assert((object)assemblySymbol != null);
            _targetAssembly = assemblySymbol;
        }

        public AssemblyIdentity MetadataIdentity => _targetAssembly.Identity;

        public override string ToString()
        {
            return _targetAssembly.ToString();
        }

        #region Cci.IAssemblyReference

        void Cci.IReference.Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        AssemblyIdentity Cci.IAssemblyReference.Identity => MetadataIdentity;

        Version Cci.IAssemblyReference.AssemblyVersionPattern
        {
            get { return MetadataIdentity.Version; }
        }

        string Cci.INamedEntity.Name
        {
            get { return MetadataIdentity.Name; }
        }

        Cci.IAssemblyReference Cci.IModuleReference.GetContainingAssembly(EmitContext context)
        {
            return this;
        }

        IEnumerable<Cci.ICustomAttribute> Cci.IReference.GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<Cci.ICustomAttribute>();
        }

        Cci.IDefinition Cci.IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        public ISymbolInternal? GetInternalSymbol()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}