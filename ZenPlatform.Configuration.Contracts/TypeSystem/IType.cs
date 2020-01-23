using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IType
    {
        Guid Id { get; set; }
        Guid? ParentId { get; set; }
        uint SystemId { get; set; }
        string Name { get; set; }
        bool IsLink { get; set; }
        bool IsObject { get; set; }
        bool IsManager { get; set; }
        bool IsDto { get; set; }
        bool IsPrimitive { get; set; }
        bool IsValue { get; set; }
        bool IsSizable { get; set; }
        bool IsScalePrecision { get; set; }
        bool IsCodeAccess { get; set; }
        bool IsTypeSpec { get; }
        IMetadata Metadata { get; set; }
        object Bag { get; set; }
        IEnumerable<IProperty> Properties { get; }
        IEnumerable<ITable> Tables { get; }
        ITypeSpec GetSpec();
    }
}