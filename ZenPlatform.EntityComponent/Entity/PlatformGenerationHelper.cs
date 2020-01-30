using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.EntityComponent.Entity
{
    public static class PlatformGenerationHelper
    {
        public static IType ConvertType(this ZenPlatform.Configuration.Contracts.TypeSystem.IType pt,
            SystemTypeBindings sb)
        {
            if (pt.IsTypeSpec)
                return ConvertType(pt.GetBase(), sb);

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