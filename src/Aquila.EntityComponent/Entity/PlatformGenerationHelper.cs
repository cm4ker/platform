using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.Data;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.EntityComponent.Entity
{
    public static class PlatformGenerationHelper
    {
        public static RoslynType ConvertType(this IPType pt,
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