using Aquila.Core.Querying.Model;
using Microsoft.IdentityModel.Protocols;

namespace Aquila.Core.Querying
{
    public static class QLangExtensions
    {
        public const string DbNameProperty = "DbName";
        public const string ParamCountComplexHidden = "Prm_Cx_Hidden";

        public static void SetDbName(this QLangElement item, string name)
        {
            item.SetProp(DbNameProperty, name);
        }

        public static void SetDbNameIfEmpty(this QLangElement item, string name)
        {
            if (!item.AttachedPropery.ContainsKey(DbNameProperty))
                item.AttachedPropery[DbNameProperty] = name;
        }

        public static string GetDbName(this QLangElement item)
        {
            return item.GetProp<string>(DbNameProperty);
        }

        public static T GetProp<T>(this QLangElement item, string name)
        {
            if (item.AttachedPropery.TryGetValue(name, out var result))
                return (T)result;
            else
                return default(T);
        }

        public static void SetProp(this QLangElement item, string name, object value)
        {
            item.AttachedPropery[name] = value;
        }
    }
}