using System;
using System.Data;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
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