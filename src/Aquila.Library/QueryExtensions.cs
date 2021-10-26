﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Collections.Generic;
using Aquila.Core;
using Aquila.Core.Authentication;
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
        public static string get_user_name(AqContext context)
        {
            return context.User;
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

    [ExtensionAq]
    public static class SystemExtensions
    {
        public static string get_tmp_path()
        {
            return Path.GetTempPath();
        }
    }
}