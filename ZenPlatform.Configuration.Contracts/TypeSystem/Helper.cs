using System;
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

        public static bool IsAssignableFrom(this IType typeA, IType typeB)
        {
            if (typeA.BaseId == null)
                return false;


            if (typeA.BaseId == typeB.Id)
                return true;

            return typeA.TypeManager.FindType(typeA.BaseId.Value)?.IsAssignableFrom(typeB) ?? false;
        }
    }
}