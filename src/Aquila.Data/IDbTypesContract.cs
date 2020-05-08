using System;
using System.Data.SqlTypes;

namespace Aquila.Data
{
    public interface IDbTypesContract
    {
        IDateTime DateTime { get; }

        IInt Int { get; }

        IString String { get; }

        IBool Boolean { get; }

        IGuid Guid { get; }

        IBinary Binary { get; }

        INumeric Numeric { get; }
    }

    public interface INumeric
    {
        double DefaultValue { get; }
    }

    public interface IBinary
    {
        byte[] DefaultValue { get; }
    }

    public interface IGuid
    {
        Guid DefaultValue { get; }
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
            Numeric = new SqlServerNumeric();
            Guid = new SqlServerGuid();
            Binary = new SqlServerBinary();
        }

        private class SqlServerDateTime : IDateTime
        {
            public DateTime MinValue => SqlDateTime.MinValue.Value;

            public DateTime MaxValue => SqlDateTime.MaxValue.Value;

            public DateTime DefaultValue => SqlDateTime.MinValue.Value;
        }


        private class SqlServerGuid : IGuid
        {
            public Guid DefaultValue => System.Guid.Empty;
        }


        private class SqlServerBinary : IBinary
        {
            public byte[] DefaultValue => new byte[0];
        }

        private class SqlServerNumeric : INumeric
        {
            public double DefaultValue => 0;
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
        public IGuid Guid { get; }
        public IBinary Binary { get; }
        public INumeric Numeric { get; }
    }
}