using System;
using System.Xml.Serialization;
using DbType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Primitive
{
    public abstract class XCPremitiveType : XCTypeBase
    {
        public abstract int ColumnSize { get; set; }

        public abstract int Precision { get; set; }

        public abstract int Scale { get; set; }

        public abstract bool IsNullable { get; set; }

        [XmlIgnore] public abstract DbType DBType { get; }

        [XmlIgnore] public abstract Type CLRType { get; }
    }
}