using System;
using Aquila.Core;

namespace Aquila.Library
{
    [ExtensionAq]
    public static class QueryExtensions
    {
        public static PlatformQuery query()
        {
            return new PlatformQuery();
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
}