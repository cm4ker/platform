using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IPType : ITypeManagerProvider
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

        bool IsAbstract { get; }

        PrimitiveKind PrimitiveKind { get; }

        bool IsValue { get; set; }

        bool IsSizable { get; set; }

        bool IsScalePrecision { get; set; }

        bool IsAsmAvaliable { get; set; }

        bool IsQueryAvaliable { get; set; }

        bool IsTypeSpec { get; }
        object Bag { get; set; }

        IEnumerable<IPProperty> Properties { get; }

        IEnumerable<ITable> Tables { get; }
        Guid ComponentId { get; set; }

        IPTypeSpec GetSpec();
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