using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.Data;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public static class TypeManagerHelper
    {
        public static IComponent GetComponent(this IPType ipType)
        {
            return FindComponent(ipType.TypeManager, ipType.ComponentId);
        }

        public static IPType GetBase(this IPType ipType)
        {
            return FindType(ipType.TypeManager, ipType.BaseId ?? Guid.Empty);
        }

        public static string GetNamespace(this IPType ipType)
        {
            return ipType.GetComponent().GetCodeRuleExpression(CodeGenRuleType.NamespaceRule);
        }

        public static IComponent FindComponent(this ITypeManager tm, Guid componentId)
        {
            return tm.Components.FirstOrDefault(x => x.Id == componentId);
        }

        public static IComponent FindComponentByName(this ITypeManager tm, string name)
        {
            return tm.Components.FirstOrDefault(x => x.Name == name);
        }

        public static IPType FindTypeByName(this IComponent com, string name)
        {
            return com.TypeManager.Types.FirstOrDefault(x => x.ComponentId == com.Id && x.Name == name);
        }

        public static IPProperty FindPropertyByName(this IPType ipType, string name)
        {
            return ipType.Properties.FirstOrDefault(x => x.Name == name);
        }

        public static IPType FindType(this ITypeManager tm, Guid typeId)
        {
            return tm.Types.FirstOrDefault(x => x.Id == typeId);
        }

        public static IEnumerable<IPType> GetTypes(this IComponent com)
        {
            return com.TypeManager.Types.Where(x => x.ComponentId == com.Id);
        }

        public static bool IsAssignableFrom(this IPType ipTypeA, IPType ipTypeB)
        {
            if (ipTypeA.BaseId == null)
                return false;


            if (ipTypeA.BaseId == ipTypeB.Id)
                return true;

            return ipTypeA.TypeManager.FindType(ipTypeA.BaseId.Value)?.IsAssignableFrom(ipTypeB) ?? false;
        }

        public static IEnumerable<ColumnSchemaDefinition> GetPropertySchemas(this ITypeManager manager,
            string propName, IEnumerable<IPType> types)
        {
            if (string.IsNullOrEmpty(propName)) throw new ArgumentNullException(nameof(propName));

            var internalTypes = types.ToList();

            var done = false;

            if (internalTypes.Count() == 1)
                yield return new ColumnSchemaDefinition(ColumnSchemaType.NoSpecial, internalTypes[0], propName);

            if (internalTypes.Count() > 1)
            {
                yield return new ColumnSchemaDefinition(ColumnSchemaType.Type, manager.Int, propName,
                    "", "_Type");

                foreach (var type in internalTypes)
                {
                    if (type.IsPrimitive)
                        yield return new ColumnSchemaDefinition(ColumnSchemaType.Value, type,
                            propName, "", $"_{type.Name}");

                    if (!type.IsPrimitive && !done)
                    {
                        yield return new ColumnSchemaDefinition(ColumnSchemaType.Ref, manager.Guid,
                            propName, "", "_Ref");

                        done = true;
                    }
                }
            }
        }

        public static IObjectSetting GetSettings(this IPProperty prop)
        {
            return prop.TypeManager.Settings.FirstOrDefault(x => x.ObjectId == prop.Id);
        }


        public static IObjectSetting GetSettings(this IPType ipType)
        {
            return ipType.TypeManager.Settings.FirstOrDefault(x => x.ObjectId == ipType.Id);
        }

        public static IObjectSetting GetSettings(this ITable table)
        {
            return table.TypeManager.Settings.FirstOrDefault(x => x.ObjectId == table.Id);
        }

        public static IEnumerable<ColumnSchemaDefinition> GetDbSchema(this IPProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.GetSettings().DatabaseName, prop.Types.ToList());
        }

        public static IEnumerable<ColumnSchemaDefinition> GetObjSchema(this IPProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.Name, prop.Types.ToList());
        }


        public static string ConvertToDbType(this IPType ipType)
        {
            if (ipType != null)
            {
                if (ipType.IsPrimitive)
                {
                    return ipType.PrimitiveKind switch
                    {
                        PrimitiveKind.Binary => $"varbinary({((IPTypeSpec) ipType).Size})",
                        PrimitiveKind.Guid => $"guid",
                        PrimitiveKind.Int => $"int",
                        PrimitiveKind.Numeric =>
                        $"numeric({((IPTypeSpec) ipType).Scale}, {((IPTypeSpec) ipType).Precision})",
                        PrimitiveKind.DateTime => $"datetime",
                        PrimitiveKind.Boolean => $"bool",
                        PrimitiveKind.String => $"varchar({((IPTypeSpec) ipType).Size})",
                    };
                }

                if (ipType.IsObject) return "guid";
            }


            throw new Exception("Unknown type");
        }
    }
}