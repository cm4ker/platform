using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Configuration.Common
{
    public static class PropertyHelper
    {
        public static IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(ITypeManager manager, 
            string propName, IList<IType> types)
        {
            if (string.IsNullOrEmpty(propName)) throw new ArgumentNullException(nameof(propName));

            var done = false;

            if (types.Count == 1)
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, types[0], propName);
            if (types.Count > 1)
            {
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Type, manager.Int, propName,
                    "", "_Type");

                foreach (var type in types)
                {
                    if (type.IsPrimitive)
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Value, type,
                            propName, "", $"_{type.Name}");

                    if (!type.IsPrimitive && !done)
                    {
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, manager.Guid,
                            propName, "", "_Ref");

                        done = true;
                    }
                }
            }
        }
    }
}