using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.Metadata;
using Aquila.Migrations;

namespace Aquila.Runtime
{
    public static class DRContextHelper
    {
        public static IEnumerable<ColumnSchemaDefinition> GetPropertySchemas(
            string propName, IEnumerable<SMType> types)
        {
            if (string.IsNullOrEmpty(propName)) throw new ArgumentNullException(nameof(propName));

            var hasRefAlready = false;

            foreach (var typeInfo in types.GetOrderedFlattenTypes())
            {
                ColumnSchemaType coltype;

                if (typeInfo.isComplex)
                    if (typeInfo.isType)
                        coltype = ColumnSchemaType.Type;
                    else if (typeInfo.type.IsReference)
                        coltype = ColumnSchemaType.Ref;
                    else
                        coltype = ColumnSchemaType.Value;
                else
                    coltype = ColumnSchemaType.NoSpecial;

                if (typeInfo.type.IsReference && hasRefAlready)
                    continue;

                if (typeInfo.type.IsReference)
                    hasRefAlready = true;

                yield return new ColumnSchemaDefinition(coltype, propName, typeInfo.type, typeInfo.prefix,
                    typeInfo.postfix);

                if (typeInfo.type.IsUnknown)
                    throw new Exception("Unknown metadata. This must never be happen.");
            }
        }

        public static IEnumerable<ColumnSchemaDefinition> GetOrCreateSchema(this SMProperty prop,
            EntityMigratorDataContext context)
        {
            var mdId = prop.FullName;

            var descriptor = context.RuntimeContext.FindEntityDescriptor(mdId);

            if (descriptor == null)
            {
                descriptor = context.RuntimeContext.CreateDescriptor(context.ConnectionContext);
                descriptor.DatabaseName = $"Fld_{descriptor.DatabaseId}";
                descriptor.MetadataId = mdId;
                context.RuntimeContext.Save(context.ConnectionContext);
            }

            return GetPropertySchemas(descriptor.DatabaseName, prop.Types);
        }


        public static IEnumerable<ColumnSchemaDefinition> GetSchema(this SMProperty prop,
            DatabaseRuntimeContext drContext)
        {
            var mdId = prop.FullName;

            var descriptor = drContext.FindEntityDescriptor(mdId);

            if (descriptor == null)
            {
                throw new Exception($"Descriptor with name {mdId} not found.Migrate metadata first!");
            }

            return GetPropertySchemas(descriptor.DatabaseName, prop.Types);
        }


        public static EntityDescriptor GetDescriptor(this SMProperty prop, DatabaseRuntimeContext runtimeContext)
        {
            var mdId = prop.FullName;
            var descriptor = runtimeContext.FindEntityDescriptor(mdId);

            return descriptor;
        }

        public static EntityDescriptor GetDescriptor(this SMEntity metadata, DatabaseRuntimeContext runtimeContext)
        {
            var mdId = metadata.FullName;
            var descriptor = runtimeContext.FindEntityDescriptor(mdId);
            return descriptor;
        }

        public static string GetDbTypeDescription(this SMType ipType)
        {
            if (ipType != null)
            {
                if (ipType.IsPrimitive)
                {
                    return ipType.Kind switch
                    {
                        SMTypeKind.Binary => $"varbinary({ipType.Size})",
                        SMTypeKind.Guid => $"guid",
                        SMTypeKind.Int => $"int",
                        SMTypeKind.Numeric =>
                            $"numeric({ipType.Scale}, {ipType.Precision})",
                        SMTypeKind.DateTime => $"datetime",
                        SMTypeKind.Bool => $"bool",
                        SMTypeKind.String => $"varchar({ipType.Size})",
                    };
                }

                if (ipType.IsReference) return "guid";
            }


            throw new Exception("Unknown type");
        }

        public static int GetTypeId(this SMType type, DatabaseRuntimeContext drContext)
        {
            if (type.IsPrimitive)
                return (int)type.Kind;
            if (type.IsReference)
                return type.GetSemantic().GetDescriptor(drContext).DatabaseId + (int)SMTypeKind.Reference;

            throw new Exception($"Can't get typeId for type {type.Name}");
        }
    }
}