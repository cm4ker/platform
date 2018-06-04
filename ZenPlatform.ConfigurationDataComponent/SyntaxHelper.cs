using System;
using System.Linq;
using System.Text;

namespace ZenPlatform.DataComponent
{
    public static class SyntaxHelper
    {
        public static string CSharpName(this Type type)
        {
            var sb = new StringBuilder();
            var name = type.Name;
            if (!type.IsGenericType) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                .Select(t => CSharpName(t))));
            sb.Append(">");
            return sb.ToString();
        }
    }
}