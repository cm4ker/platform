using System;
using System.Data.SqlTypes;

namespace ZenPlatform.Data
{
    public interface IDbTypesContract
    {
        IDateTime DateTime { get; }

        IInt Int { get; }

        IString String { get; }

        IBool Boolean { get; }
    }

    public interface IBool
    {
        bool DefaultValue { get; }
    }

    public interface IString
    {
        string DefaultValue { get; }
    }

    public interface IInt
    {
        int MinValue { get; }

        int MaxValue { get; }

        int DefaultValue { get; }
    }

    public interface IDateTime
    {
        DateTime MinValue { get; }

        DateTime MaxValue { get; }

        DateTime DefaultValue { get; }
    }

    public class SqlServerDbTypesContract : IDbTypesContract
    {
        public SqlServerDbTypesContract()
        {
            DateTime = new SqlServerDateTime();
            Int = new SqlServerInt();
            String = new SqlServerString();
            Boolean = new SqlServerBoolean();
        }

        private class SqlServerDateTime : IDateTime
        {
            public DateTime MinValue => SqlDateTime.MinValue.Value;

            public DateTime MaxValue => SqlDateTime.MaxValue.Value;

            public DateTime DefaultValue => SqlDateTime.MinValue.Value;
        }


        private class SqlServerInt : IInt
        {
            public int MinValue => int.MinValue;

            public int MaxValue => int.MaxValue;

            public int DefaultValue => 0;
        }

        private class SqlServerString : IString
        {
            public string DefaultValue => "";
        }

        private class SqlServerBoolean : IBool
        {
            public bool DefaultValue => false;
        }

        public IDateTime DateTime { get; }

        public IInt Int { get; }

        public IString String { get; }
        public IBool Boolean { get; }
    }
}