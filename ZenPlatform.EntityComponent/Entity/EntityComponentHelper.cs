using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.EntityComponent.Entity
{
    public static class EntityComponentHelper
    {
        public static IPType GetDtoType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsDto && x.GroupId == ipType.GroupId);
        }

        public static IPType GetManagerType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsManager && x.GroupId == ipType.GroupId);
        }

        public static IPType GetLinkType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsLink && x.GroupId == ipType.GroupId);
        }

        public static IPType GetObjectType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsObject && x.GroupId == ipType.GroupId);
        }

        public static T GetMD<T>(this IPType type)
        {
            return (T) type.TypeManager.Metadatas.FirstOrDefault(x => x.Id == type.GroupId)?.Metadata;
        }

        public static string GetTableDtoRowClassFullName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId).GetDtoType();
            return $"{type.GetNamespace()}.{table.GetDtoRowClassName()}";
        }

        public static string GetDtoRowClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId).GetDtoType();
            return $"RowDto{type.Name}_{table.Name}";
        }

        public static string GetObjectRowFullClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId);
            return $"{type.GetNamespace()}.{table.GetObjectRowClassName()}";
        }
        
        public static string GetObjectRowClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId);
            return $"RWRowDtoWrapper{type.Name}_{table.Name}";
        }
        
        public static string GetLinkRowFullClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId);
            return $"{type.GetNamespace()}.{table.GetLinkRowClassName()}";
        }
        
        public static string GetLinkRowClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId);
            return $"RWRowDtoWrapper{type.Name}_{table.Name}";
        }
        
        public static string GetObjectRowCollectionClassName(this ITable table)
        {
            var type = table.TypeManager.FindType(table.ParentId);
            return $"RWRowCollection{type.Name}_{table.Name}";
        }
    }
}