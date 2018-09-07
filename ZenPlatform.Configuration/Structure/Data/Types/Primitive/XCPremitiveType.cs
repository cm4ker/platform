using System;
using System.Data;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    /// ����������� ��� ������
    /// </summary>
    public abstract class XCPremitiveType : XCTypeBase
    {
        /// <summary>
        /// ������ �������
        /// </summary>
        public abstract int ColumnSize { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public abstract int Precision { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        public abstract int Scale { get; set; }

        /// <summary>
        /// ����� ���� ����� NULL
        /// </summary>
        public abstract bool IsNullable { get; set; }

        /// <summary>
        /// ��� ���� ������
        /// </summary>
        [XmlIgnore] public abstract DbType DBType { get; }

        /// <summary>
        /// ��������������� ��� CLR
        /// </summary>
        [XmlIgnore] public abstract Type CLRType { get; }
    }
}