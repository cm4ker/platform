using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    public static class QLangExtensions
    {
        public static void SetDbName(this QLangElement item, string name)
        {
            item.AttachedPropery["DbName"] = name;
        }

        public static void SetDbNameIfEmpty(this QLangElement item, string name)
        {
            if (!item.AttachedPropery.ContainsKey("DbName"))
                item.AttachedPropery["DbName"] = name;
        }

        public static string GetDbName(this QLangElement item)
        {
            if (item.AttachedPropery.TryGetValue("DbName", out var result))
                return (string) result;
            else
                return null;
        }
    }
}