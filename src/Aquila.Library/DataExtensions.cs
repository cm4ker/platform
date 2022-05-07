using System;
using System.Collections.Generic;
using System.IO;
using Aquila.Core;

namespace Aquila.Library
{
    [ExtensionAq]
    public static class DataExtensions
    {
        public static AqQuery query(AqContext context)
        {
            return new AqQuery(context);
        }

        public static void begin_tran(AqContext context)
        {
            context.DataContext.BeginTransaction();
        }

        public static void commit_tran(AqContext context)
        {
            context.DataContext.CommitTransaction();
        }

        public static void rollback(AqContext context)
        {
            context.DataContext.RollbackTransaction();
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

        public static AqException exception(AqContext ctx)
        {
            return new AqException();
        }

        public static AqException exception(AqContext ctx, string message)
        {
            return new AqException(message);
        }

        public static void print(AqContext ctx, object obj)
        {
            ctx.OutputText.Write(obj);
        }

        public static void print(AqContext ctx, string obj)
        {
            ctx.OutputText.Write(obj);
        }

        public static void print(AqContext ctx, int obj)
        {
            ctx.OutputText.Write(obj);
        }

        public static void println(AqContext ctx, object obj)
        {
            ctx.OutputText.WriteLine(obj);
        }

        public static void println(AqContext ctx, int obj)
        {
            ctx.OutputText.WriteLine(obj);
        }
    }
}