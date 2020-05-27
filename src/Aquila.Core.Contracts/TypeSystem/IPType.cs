using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPType : IPUniqueObject
    {
        Guid? BaseId { get; }

        Guid? GroupId { get; }

        string Name { get; }

        bool IsDbAffect { get; }

        bool IsPrimitive { get; }

        bool IsAbstract { get; }

        PrimitiveKind PrimitiveKind { get; }

        bool IsValue { get; set; }

        bool IsSizable { get; set; }

        bool IsScalePrecision { get; set; }

        bool IsAsmAvailable { get; set; }

        bool IsQueryAvailable { get; set; }

        bool IsTypeSpec { get; }

        bool IsTypeSet { get; }

        bool IsArray { get; }

        object Bag { get; set; }

        IEnumerable<IPProperty> Properties { get; }

        IEnumerable<IPInvokable> Methods { get; }

        Guid ComponentId { get; set; }

        IPTypeSpec GetSpec();
    }

    public enum PrimitiveKind
    {
        Unknown,
        String,
        Int,
        Binary,
        Boolean,
        DateTime,
        Guid,
        Numeric,
    }
}