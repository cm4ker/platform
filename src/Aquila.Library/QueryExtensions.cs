using System;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;
using Aquila.Core;

namespace Aquila.Library
{
    [ExtensionAq]
    public static class QueryExtensions
    {
        public static AqQuery query()
        {
            return new AqQuery();
        }
    }

    [ExtensionAq]
    public static class DateExtensions
    {
        public static DateTime get_date()
        {
            return DateTime.Now;
        }
    }

    [ExtensionAq]
    public static class ListExtensions
    {
        public static List<object> list()
        {
            return new List<object>();
        }

        public static List<T> list<T>()
        {
            return new List<T>();
        }
    }
}