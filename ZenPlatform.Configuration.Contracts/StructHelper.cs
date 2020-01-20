using System;
using System.Linq;

namespace ZenPlatform.Configuration.Contracts
{
    public static class StructHelper
    {
        public static IXCLinkType GetLink(this IXCObjectType type)
        {
            return (IXCLinkType) type.Parent.Types.FirstOrDefault(x =>
                x is IXCLinkType l && l.ParentType == type);
        }


        public static IXCProperty GetPropertyByName(this IXCStructureType type, string propName)
        {
            if (type.HasProperties)
                return type.GetProperties().FirstOrDefault(x => x.Name == propName) ??
                       throw new Exception($"Property not found: {propName}");
            else
                throw new Exception($"Component not support properties: {type.Parent.Info.ComponentName}");
        }
    }
}