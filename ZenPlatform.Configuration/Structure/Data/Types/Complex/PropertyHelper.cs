using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public static class PropertyHelper
    {
        public static IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(
            string propName, IList<IXCType> types)
        {
            if (string.IsNullOrEmpty(propName)) throw new ArgumentNullException(nameof(propName));

            var done = false;

            if (types.Count == 1)
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, types[0], propName, false);
            if (types.Count > 1)
            {
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Type, new XCInt(), propName,
                    false, "", "_Type");

                foreach (var type in types)
                {
                    if (type is XCPrimitiveType)
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Value, type,
                            propName, false, "", $"_{type.Name}");

                    if (type is XCObjectTypeBase obj && !done)
                    {
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, PlatformTypesFactory.Guid, propName,
                            !obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn, "", "_Ref");

                        done = true;
                    }
                }
            }
        }
    }
}