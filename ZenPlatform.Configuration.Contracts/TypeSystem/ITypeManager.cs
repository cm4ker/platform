using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITypeManager
    {
        IPType Int { get; }
        IPType DateTime { get; }
        IPType Binary { get; }
        IPType String { get; }
        IPType Boolean { get; }
        IPType Guid { get; }
        IPType Numeric { get; }

        IReadOnlyList<IPType> Types { get; }
        IReadOnlyList<IPProperty> Properties { get; }
        IReadOnlyList<ITable> Tables { get; }
        IReadOnlyList<IComponent> Components { get; }
        IReadOnlyList<IObjectSetting> Settings { get; }

        IReadOnlyList<IMetadataRow> Metadatas { get; }

        void Register(IPType ipType);
        void Register(IPProperty p);
        void Register(IPropertyType type);
        void Register(IComponent component);
        void Register(ITable table);
        
        IPType Type();
        IPTypeSpec Type(IPType ipType);
        IPProperty Property();
        IPropertyType PropertyType();
        ITable Table();

        void AddMD(Guid id, Guid parentId, object metadata);

        void Verify();

        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
        void AddOrUpdateSetting(IObjectSetting setting);
    }
}