using System;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Component.Shared
{
    public static class PlatformGenerationHelper
    {
        public static RoslynType ConvertType(this IPType pt,
            SystemTypeBindings sb)
        {
            if (pt.IsTypeSet)
                throw new Exception("We can't convert set out types to CLR type");

            if (pt.IsTypeSpec)
            {
                if (pt.IsArray)
                {
                    return sb.List.MakeGenericType(pt.GetBase().ConvertType(sb));
                }
                else
                    return ConvertType(pt.GetBase(), sb);
            }

            if (pt.IsPrimitive)
                return pt.PrimitiveKind switch
                {
                    PrimitiveKind.Binary => sb.Byte.MakeArrayType(),
                    PrimitiveKind.Int => sb.Int,
                    PrimitiveKind.String => sb.String,
                    PrimitiveKind.Numeric => sb.Double,
                    PrimitiveKind.Boolean => sb.Boolean,
                    PrimitiveKind.DateTime => sb.DateTime,
                    PrimitiveKind.Guid => sb.Guid,
                };

            return sb.TypeSystem.FindType(pt.GetNamespace() + "." + pt.Name);
        }
    }
}