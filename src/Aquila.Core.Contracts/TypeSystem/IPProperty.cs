using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPField : IPMember
    {
    }

    public interface IPFieldBuilder : IPField
    {
        
    }


    public interface IPProperty : IPMember
    {
        bool IsSelfLink { get; }
        bool IsSystem { get; }
        bool IsUnique { get; }
        bool IsReadOnly { get; }
        IPType Type { get; }

        public IPMethod Getter { get; }

        public IPMethod Setter { get; }
    }


    public interface IPPropertyBuilder : IPProperty
    {
        void SetType(Guid guid);

        void SetIsSelfLink(bool value);

        void SetIsSystem(bool value);

        void SetIsUnique(bool value);

        void SetIsReadOnly(bool value);

        void SetGetter(Guid id);

        void SetSetter(Guid id);
    }


    public interface IPMethodBuilder : IPMethod
    {
    }

    public interface IPUniqueObject : ITypeManagerProvider
    {
        Guid Id { get; }
    }

    public interface IPMember : IPUniqueObject
    {
        Guid ParentId { get; }

        string Name { get; set; }

        bool IsProperty { get; }

        bool IsMethod { get; }

        bool IsConstructor { get; }
    }
}