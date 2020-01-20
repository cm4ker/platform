using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    /// Примитивный тип данных
    /// </summary>
    public abstract class XCPrimitiveType : XCTypeBase, IXCPrimitiveType
    {
        /// <summary>
        /// Может быть равна NULL
        /// </summary>
        public bool IsNullable { get; set; }
    }
}