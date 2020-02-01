using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IType : ITypeManagerProvider
    {
        Guid Id { get; set; }

        Guid? BaseId { get; set; }

        Guid? GroupId { get; set; }

        uint SystemId { get; set; }

        string Name { get; set; }

        bool IsLink { get; set; }

        bool IsObject { get; set; }

        bool IsManager { get; set; }

        bool IsDto { get; set; }

        bool IsPrimitive { get; }

        PrimitiveKind PrimitiveKind { get; }

        bool IsValue { get; set; }

        bool IsSizable { get; set; }

        bool IsScalePrecision { get; set; }

        bool IsCodeAvaliable { get; set; }

        bool IsQueryAvaliable { get; set; }

        bool IsTypeSpec { get; }
        object Bag { get; set; }

        IEnumerable<IProperty> Properties { get; }

        IEnumerable<ITable> Tables { get; }
        Guid ComponentId { get; set; }

        ITypeSpec GetSpec();
    }

    public enum PrimitiveKind
    {
        None,
        String,
        Int,
        Binary,
        Boolean,
        DateTime,
        Guid,
        Numeric,
    }
}