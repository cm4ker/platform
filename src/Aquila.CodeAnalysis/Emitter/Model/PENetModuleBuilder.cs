using System;
using System.Collections.Generic;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis.Emit;
using Roslyn.Utilities;
using Microsoft.CodeAnalysis;
using Cci = Microsoft.Cci;

namespace Pchp.CodeAnalysis.Emit
{
    internal sealed class PENetModuleBuilder : PEModuleBuilder
    {
        internal PENetModuleBuilder(
            PhpCompilation compilation,
            IModuleSymbol sourceModule,
            EmitOptions emitOptions,
            Cci.ModulePropertiesForSerialization serializationProperties,
            IEnumerable<ResourceDescription> manifestResources)
            : base(compilation, (SourceModuleSymbol)sourceModule, serializationProperties, manifestResources, OutputKind.NetModule, emitOptions)
        {
        }

        public override IEnumerable<Cci.IFileReference> GetFiles(EmitContext context) => SpecializedCollections.EmptyEnumerable<Cci.IFileReference>();

        public override ISourceAssemblySymbolInternal SourceAssemblyOpt => null;
    }
}
