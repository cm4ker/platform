using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading;
using Aquila.CodeAnalysis.Symbols.PE;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    internal enum ObsoleteDiagnosticKind
    {
        NotObsolete,
        Suppressed,
        Diagnostic,
        Lazy,
        LazyPotentiallySuppressed,
    }

    internal static class ObsoleteAttributeHelpers
    {
        /// <summary>
        /// Initialize the ObsoleteAttributeData by fetching attributes and decoding ObsoleteAttributeData. This can be 
        /// done for Metadata symbol easily whereas trying to do this for source symbols could result in cycles.
        /// </summary>
        internal static void InitializeObsoleteDataFromMetadata(ref ObsoleteAttributeData data, EntityHandle token,
            PEModuleSymbol containingModule, bool ignoreByRefLikeMarker)
        {
            if (ReferenceEquals(data, ObsoleteAttributeData.Uninitialized))
            {
                ObsoleteAttributeData obsoleteAttributeData =
                    GetObsoleteDataFromMetadata(token, containingModule, ignoreByRefLikeMarker);
                Interlocked.CompareExchange(ref data, obsoleteAttributeData, ObsoleteAttributeData.Uninitialized);
            }
        }

        /// <summary>
        /// Get the ObsoleteAttributeData by fetching attributes and decoding ObsoleteAttributeData. This can be 
        /// done for Metadata symbol easily whereas trying to do this for source symbols could result in cycles.
        /// </summary>
        internal static ObsoleteAttributeData GetObsoleteDataFromMetadata(EntityHandle token,
            PEModuleSymbol containingModule, bool ignoreByRefLikeMarker)
        {
            ObsoleteAttributeData obsoleteAttributeData;
            obsoleteAttributeData =
                containingModule.Module.TryGetDeprecatedOrExperimentalOrObsoleteAttribute(token,
                    new MetadataDecoder(containingModule), ignoreByRefLikeMarker);
            Debug.Assert(obsoleteAttributeData == null || !obsoleteAttributeData.IsUninitialized);
            return obsoleteAttributeData;
        }
    }
}