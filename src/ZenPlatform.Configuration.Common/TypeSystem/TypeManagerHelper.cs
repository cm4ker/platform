using System.Data;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public static class TypeManagerHelper
    {
        public static IPType CreateUX(this ITypeManager tm)
        {
            var result = tm.Type();
            result.IsUX = true;

            return result;
        }
        
        public static T GetMD<T>(this IPType type)
        {
            return (T) type.TypeManager.Metadatas.FirstOrDefault(x => x.Id == type.GroupId)?.Metadata;
        }
    }
    
    
}