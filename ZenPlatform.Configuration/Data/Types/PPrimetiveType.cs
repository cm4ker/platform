using System;
using System.Data;
using DbType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PPrimetiveType : PTypeBase
    {
        public abstract int ColumnSize { get; set; }

        public abstract int Precision { get; set; }

        public abstract int Scale { get; set; }

        public abstract bool IsNullable { get; set; }

        public abstract DbType DBType { get; }
        public abstract Type CLRType { get; }
    }
}