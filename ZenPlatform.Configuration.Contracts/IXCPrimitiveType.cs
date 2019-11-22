using System;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCPrimitiveType : IXCType
    {
        /// <summary>
        /// Может быть равна NULL
        /// </summary>
        bool IsNullable { get; set; }
    }
}