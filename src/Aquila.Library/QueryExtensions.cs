using System;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;
using Aquila.Core;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Library
{
    [ExtensionAq]
    public static class QueryExtensions
    {
        public static AqQuery query(AqContext context)
        {
            return new AqQuery(context);
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
    public static class UserExtensions
    {
        public static IUser get_user(AqContext context)
        {
            return context.Session.User;
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