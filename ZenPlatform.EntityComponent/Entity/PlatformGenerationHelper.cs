using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.EntityComponent.Entity
{
    public static class PlatformGenerationHelper
    {
        public static IType ConvertType(this IXCType pt, SystemTypeBindings sb)
        {
            return pt switch
            {
                XCBinary b => sb.Byte.MakeArrayType(),
                XCInt b => sb.Int,
                XCString b => sb.String,
                XCNumeric b => sb.Double,
                XCBoolean b => sb.Boolean,
                XCDateTime b => sb.DateTime,
                XCGuid b => sb.Guid,

                IXCLinkType b => sb.TypeSystem.FindType(
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name),
                IXCObjectType b => sb.TypeSystem.FindType(
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name),
            };
        }
    }
}