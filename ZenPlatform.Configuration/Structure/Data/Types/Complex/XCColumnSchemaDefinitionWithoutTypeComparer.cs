using System.Collections.Generic;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public class XCColumnSchemaDefinitionWithoutTypeComparer : IEqualityComparer<XCColumnSchemaDefinition>
    {
        public bool Equals(XCColumnSchemaDefinition x, XCColumnSchemaDefinition y)
        {
            return !(
                x != null && y != null
                          && x.FullName.Equals(y.FullName)
                          //  && x.PlatformType.Equals(y.PlatformType)
                          && x.SchemaType == y.SchemaType
            );
        }

        public int GetHashCode(XCColumnSchemaDefinition obj)
        {
            return obj.FullName.GetHashCode() ^ obj.SchemaType.GetHashCode();
        }
    }
}