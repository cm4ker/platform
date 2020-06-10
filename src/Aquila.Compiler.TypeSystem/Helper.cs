using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Aqua;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Compiler.Contracts;

namespace Aquila.Core.Contracts.TypeSystem
{
    public static class TypeManagerHelper
    {
        public static PType CreateUX(this TypeManager tm)
        {
            var result = tm.DefineType();
            return result;
        }

        public static T GetMD<T>(this PType type)
        {
            return (T) type.TypeManager.Metadatas.FirstOrDefault(x => x.Id == type.GroupId)?.Metadata;
        }

        public static Component GetComponent(this PType PType)
        {
            if (PType.ComponentId != null)
                return FindComponent(PType.TypeManager, PType.ComponentId.Value);

            return null;
        }

        public static PType GetBase(this PType PType)
        {
            return FindType(PType.TypeManager, PType.BaseId ?? Guid.Empty);
        }

        public static string GetNamespace(this PType PType)
        {
            return PType.GetComponent().GetCodeRuleExpression(CodeGenRuleType.NamespaceRule);
        }

        public static Component FindComponent(this TypeManager tm, Guid componentId)
        {
            return tm.Components.FirstOrDefault(x => x.Id == componentId);
        }

        public static Component FindComponentByName(this TypeManager tm, string name)
        {
            return tm.Components.FirstOrDefault(x => x.Name == name);
        }

        public static PType FindTypeByName(this Component com, string name)
        {
            return com.TypeManager.Types.FirstOrDefault(x => x.ComponentId == com.Id && x.Name == name);
        }

        public static PProperty FindPropertyByName(this PType PType, string name)
        {
            return PType.Properties.FirstOrDefault(x => x.Name == name);
        }

        public static PType FindType(this TypeManager tm, Guid typeId)
        {
            return tm.Types.FirstOrDefault(x => x.Id == typeId) ?? tm.Unknown;
        }

        public static PType FindType<T>(this TypeManager tm)
        {
            var backendType = tm.Backend.FindType<T>();
            return tm.FindType(backendType.Id) ?? tm.ExportedType(backendType);
        }

        public static PField FindField(this PType type, string fieldName)
        {
            return type.Fields.FirstOrDefault(x => x.Name == fieldName);
        }

        public static PMethod FindMethod(this PType type, string methodName, params PType[] args)
        {
            return type.Methods.FirstOrDefault(x =>
            {
                return x.Name == methodName
                       && x.Parameters.Select(p => p.Type).SequenceEqual(args);
            });
        }

        public static PMethod FindMethod(this TypeManager tm, Guid methodId)
        {
            return (PMethod) tm.Methods.FirstOrDefault(x => x is PMethod && x.Id == methodId);
        }

        public static PInvokable FindInvokable(this TypeManager tm, Guid invokableId)
        {
            return null;
        }

        public static IEnumerable<PParameter> FindParameters(this TypeManager tm, Guid invokableId)
        {
            return tm.Parameters.Where(x => x.InvokableId == invokableId);
        }

        public static IEnumerable<PProperty> FindProperties(this TypeManager tm, Guid ownerId)
        {
            return null;
        }

        public static Guid HandleTypeSet(this TypeManager tm, IEnumerable<Guid> typeIds)
        {
            var listTypes = typeIds.ToList();
            if (listTypes.Count == 0)
                throw new Exception("Unknown type");

            if (listTypes.Count > 1)
            {
                var ts = tm.DefineTypeSet(listTypes);
                // tm.Register(ts);
                return ts.Id;
            }

            return listTypes.First();
        }

        public static IEnumerable<PType> GetTypes(this Component com)
        {
            return com.TypeManager.Types.Where(x => x.ComponentId == com.Id);
        }

        public static bool IsAssignableFrom(this PType PTypeA, PType PTypeB)
        {
            if (PTypeA.Id == PTypeB.Id)
                return true;

            if (PTypeA.BaseId == null)
                return false;


            if (PTypeA.BaseId == PTypeB.Id)
                return true;

            return PTypeA.TypeManager.FindType(PTypeA.BaseId.Value)?.IsAssignableFrom(PTypeB) ?? false;
        }

        public static IEnumerable<ColumnSchemaDefinition> GetPropertySchemas(this TypeManager manager,
            string propName, IEnumerable<PType> types)
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
                    {
                        var typeName = type.Name;

                        if (type is PTypeSpec)
                            typeName = type.GetBase().Name;

                        yield return new ColumnSchemaDefinition(ColumnSchemaType.Value, type,
                            propName, "", $"_{typeName}");
                    }

                    if (!type.IsPrimitive && !done)
                    {
                        yield return new ColumnSchemaDefinition(ColumnSchemaType.Ref, manager.Guid,
                            propName, "", "_Ref");

                        done = true;
                    }
                }
            }
        }

        public static ObjectSetting GetSettings(this PProperty prop)
        {
            return prop.TypeManager.Settings.FirstOrDefault(x => x.ObjectId == prop.Id);
        }


        public static ObjectSetting GetSettings(this PType PType)
        {
            if (PType.IsTypeSpec)
                return PType.GetBase().GetSettings();

            return PType.TypeManager.Settings.FirstOrDefault(x => x.ObjectId == PType.Id);
        }

        public static IEnumerable<PType> GetTypes(this PProperty prop)
        {
            return prop.Type.Unwrap();
        }

        public static IEnumerable<PType> Unwrap(this PType type)
        {
            if (type.IsTypeSet)
            {
                foreach (var retType in ((PTypeSet) type).Types)
                {
                    yield return retType;
                }
            }
            else
                yield return type;
        }

        public static IEnumerable<ColumnSchemaDefinition> GetDbSchema(this PProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.GetSettings().DatabaseName, prop.GetTypes().ToList());
        }

        public static IEnumerable<ColumnSchemaDefinition> GetObjSchema(this PProperty prop)
        {
            return GetPropertySchemas(prop.TypeManager, prop.Name, prop.GetTypes().ToList());
        }


        public static void AddType(this PTypeSet set, PType type)
        {
            set.AddType(type.Id);
        }

        public static string ConvertToDbType(this PType PType)
        {
            if (PType != null)
            {
                if (PType.IsPrimitive)
                {
                    return PType.PrimitiveKind switch
                    {
                        PrimitiveKind.Binary => $"varbinary({((PTypeSpec) PType).Size})",
                        PrimitiveKind.Guid => $"guid",
                        PrimitiveKind.Int => $"int",
                        PrimitiveKind.Numeric =>
                        $"numeric({((PTypeSpec) PType).Scale}, {((PTypeSpec) PType).Precision})",
                        PrimitiveKind.DateTime => $"datetime",
                        PrimitiveKind.Boolean => $"bool",
                        PrimitiveKind.String => $"varchar({((PTypeSpec) PType).Size})",
                    };
                }

                //  if (PType.IsObject) return "guid";
            }


            throw new Exception("Unknown type");
        }
    }
}