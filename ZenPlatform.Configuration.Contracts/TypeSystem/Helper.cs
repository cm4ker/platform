using System;
using System.Linq;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public static class TypeManagerHelper
    {
        public static IComponent FindComponent(this ITypeManager tm, Guid componentId)
        {
            return tm.Components.FirstOrDefault(x => x.Id == componentId);
        }

        public static IType FindType(this ITypeManager tm, Guid typeId)
        {
            return tm.Types.FirstOrDefault(x => x.Id == typeId);
        }
    }
}