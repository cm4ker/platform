using System;
using System.Data;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    /// Примитивный тип данных
    /// </summary>
    public abstract class XCPremitiveType : XCTypeBase
    {
        /// <summary>
        /// Размер колонки
        /// </summary>
        public abstract int ColumnSize { get; set; }

        /// <summary>
        /// Точность
        /// </summary>
        public abstract int Precision { get; set; }

        /// <summary>
        /// Значимость
        /// </summary>
        public abstract int Scale { get; set; }

        /// <summary>
        /// Может быть равна NULL
        /// </summary>
        public abstract bool IsNullable { get; set; }

        /// <summary>
        /// Тип базы данных
        /// </summary>
        [XmlIgnore] public abstract DbType DBType { get; }

        /// <summary>
        /// Соответствующий тип CLR
        /// </summary>
        [XmlIgnore] public abstract Type CLRType { get; }
    }
}