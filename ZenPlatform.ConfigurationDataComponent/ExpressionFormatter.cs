using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.DataComponent
{
    public static class NamedStringFormatterHelper
    {
        public static string NamedFormat(this string str, object values)
        {
            var props = values.GetType().GetProperties();
            string result = str;
            foreach (var prop in props)
            {
                if (result.Contains($"{{{{{prop.Name}}}}}"))
                {
                    result = result.Replace($"{{{{{prop.Name}}}}}", $"{{{prop.Name}}}");
                }

                result = result.Replace($"{{{prop.Name}}}", prop.GetValue(values).ToString());
            }

            return result;
        }
    }
}
