using System;
using System.Linq;

namespace Aquila.Core.Helpers
{
    /// <summary>
    /// Помошник для работы с типами C#
    /// </summary>
    public static class ClrTypeHelper
    {
        /// <summary>
        /// Получить тип в виде правлиьной строки 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToTypeString(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToTypeString(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }
    }
}