using Aquila.CodeAnalysis.Symbols;

namespace Aquila.Syntax.Metadata
{
    internal static class PlatformTypeExtensions
    {
        public static bool IsEntity(this TypeSymbol type)
        {
            var attr = type.GetAttribute(CoreTypes.AquilaEntityAttributeFullName);
            return attr != null;
        }

        public static bool IsLink(this TypeSymbol type)
        {
            var attr = type.GetAttribute(CoreTypes.AquilaLinkAttributeFullName);
            return attr != null;
        }
    }
}