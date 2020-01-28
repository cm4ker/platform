using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public static class TypeManagerHelper
    {
        public static IComponent GetComponent(this IType type)
        {
            return FindComponent(type.TypeManager, type.ComponentId);
        }

        public static IComponent FindComponent(this ITypeManager tm, Guid componentId)
        {
            return tm.Components.FirstOrDefault(x => x.Id == componentId);
        }

        public static IComponent FindComponentByName(this ITypeManager tm, string name)
        {
            return tm.Components.FirstOrDefault(x => x.Name == name);
        }

        public static IType FindTypeByName(this IComponent com, string name)
        {
            return com.TypeManager.Types.FirstOrDefault(x => x.ComponentId == com.Id && x.Name == name);
        }

        public static IProperty FindPropertyByName(this IType type, string name)
        {
            return type.Properties.FirstOrDefault(x => x.Name == name);
        }

        public static IType FindType(this ITypeManager tm, Guid typeId)
        {
            return tm.Types.FirstOrDefault(x => x.Id == typeId);
        }

        public static IEnumerable<IType> GetTypes(this IComponent com)
        {
            return com.TypeManager.Types.Where(x => x.ComponentId == com.Id);
        }

        public static bool IsAssignableFrom(this IType typeA, IType typeB)
        {
            if (typeA.BaseId == null)
                return false;


            if (typeA.BaseId == typeB.Id)
                return true;

            return typeA.TypeManager.FindType(typeA.BaseId.Value)?.IsAssignableFrom(typeB) ?? false;
        }

        public static IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(ITypeManager manager,
            string propName, IEnumerable<IType> types)
        {
            if (string.IsNullOrEmpty(propName)) throw new ArgumentNullException(nameof(propName));

            var internalTypes = types.ToList();

            var done = false;

            if (internalTypes.Count() == 1)
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, internalTypes[0], propName);
            if (internalTypes.Count() > 1)
            {
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Type, manager.Int, propName,
                    "", "_Type");

                foreach (var type in internalTypes)
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

        public static IEnumerable<XCColumnSchemaDefinition> GetDbSchema(this IProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.Metadata.DatabaseColumnName, prop.Types.ToList());
        }

        public static IEnumerable<XCColumnSchemaDefinition> GetObjSchema(this IProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.Name, prop.Types.ToList());
        }
    }
}