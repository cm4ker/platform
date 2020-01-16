using System;
using System.Data.SqlTypes;

namespace ZenPlatform.Data
{
    public interface IDbTypesContract
    {
        IDateTime DateTime { get; }
    }

    public interface IDateTime
    {
        DateTime MinValue { get; }

        DateTime MaxValue { get; }
    }

    public class SqlServerDbTypesContract : IDbTypesContract
    {
        public SqlServerDbTypesContract()
        {
            DateTime = new SqlServerDateTime();
        }
        
        private class SqlServerDateTime : IDateTime
        {
            public DateTime MinValue => SqlDateTime.MinValue.Value;
            public DateTime MaxValue => SqlDateTime.MaxValue.Value;
        }

        public IDateTime DateTime { get; }
    }
}