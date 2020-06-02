using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPTypeBuilder : IPType
    {
        void SetBase(Guid baseId);
        void SetComponent(Guid? componentId);
        void SetName(string name);
        void SetNamespace(string @namespace);
        void SetScope(ScopeAffects scope);
    }

    public interface IPType : IPUniqueObject
    {
        Guid? BaseId { get; }

        Guid? GroupId { get; }

        string Name { get; }

        bool IsDbAffect { get; }

        bool IsPrimitive { get; }

        bool IsAbstract { get; }

        PrimitiveKind PrimitiveKind { get; }

        bool IsValue { get; }

        bool IsSizable { get; }

        bool IsScalePrecision { get; }

        bool IsTypeSpec { get; }

        bool IsTypeSet { get; }

        bool IsArray { get; }

        object Bag { get; set; }

        ScopeAffects Scope { get; }

        IEnumerable<IPProperty> Properties { get; }

        IEnumerable<IPInvokable> Methods { get; }

        Guid? ComponentId { get; }

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