using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    internal partial class Symbol : Cci.IReference
    {
        
        internal Symbol AdaptedSymbol => this;
        internal Symbol GetCciAdapter() => this;
        
        /// <summary>
        /// Checks if this symbol is a definition and its containing module is a SourceModuleSymbol.
        /// </summary>
        [Conditional("DEBUG")]
        internal protected void CheckDefinitionInvariant()
        {
            // can't be generic instantiation
            Debug.Assert(this.IsDefinition);

            // must be declared in the module we are building
            Debug.Assert(this.ContainingModule is SourceModuleSymbol ||
                         (this.Kind == SymbolKind.Assembly && this is SourceAssemblySymbol) ||
                         (this.Kind == SymbolKind.NetModule && this is SourceModuleSymbol));
        }

        /// <summary>
        /// Return whether the symbol is either the original definition
        /// or distinct from the original. Intended for use in Debug.Assert
        /// only since it may include a deep comparison.
        /// </summary>
        internal bool IsDefinitionOrDistinct()
        {
            return this.IsDefinition || !this.Equals(this.OriginalDefinition);
        }

        Cci.IDefinition Cci.IReference.AsDefinition(EmitContext context)
        {
            throw new NotSupportedException();
        }

        public ISymbolInternal? GetInternalSymbol()
        {
            return this;
        }

        void Cci.IReference.Dispatch(Cci.MetadataVisitor visitor)
        {
            throw new NotSupportedException();
        }

        internal virtual IEnumerable<AttributeData> GetCustomAttributesToEmit(
            CommonModuleCompilationState compilationState)
        {
            return this.GetAttributes();
        }

        IEnumerable<Cci.ICustomAttribute> Cci.IReference.GetAttributes(EmitContext context)
        {
            var attrs = GetCustomAttributesToEmit(((PEModuleBuilder)context.Module)
                .CompilationState).Cast<Cci.ICustomAttribute>();

            return attrs;
        }

        public int MetadataToken { get; }

        Cci.IReference ISymbolInternal.GetCciAdapter() => this;
    }
}