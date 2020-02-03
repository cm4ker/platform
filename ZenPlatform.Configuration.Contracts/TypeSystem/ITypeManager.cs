using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITypeManager
    {
        IType Int { get; }
        IType DateTime { get; }
        IType Binary { get; }
        IType String { get; }
        IType Boolean { get; }
        IType Guid { get; }
        IType Numeric { get; }

        IReadOnlyList<IType> Types { get; }
        IReadOnlyList<IProperty> Properties { get; }
        IReadOnlyList<ITable> Tables { get; }
        IReadOnlyList<IComponent> Components { get; }
        IReadOnlyList<IObjectSetting> Settings { get; }

        IReadOnlyList<IMetadataRow> Metadatas { get; }

        void Register(IType type);
        void Register(IProperty p);
        void Register(IPropertyType type);
        void Register(IComponent component);

        IType Type();
        ITypeSpec Type(IType type);
        IProperty Property();
        IPropertyType PropertyType();
        ITable Table();

        void AddMD(Guid id, Guid parentId, object metadata);

        void Verify();

        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
    }
}