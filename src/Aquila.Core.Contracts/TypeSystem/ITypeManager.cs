using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
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

        IReadOnlyList<IPMethod> Methods { get; }

        IReadOnlyList<ITable> Tables { get; }
        IReadOnlyList<IComponent> Components { get; }
        IReadOnlyList<IObjectSetting> Settings { get; }

        IReadOnlyList<IMetadataRow> Metadatas { get; }

        void Register(IPType ipType);
        void Register(IPProperty p);
        void Register(IPMethod method);
        void Register(IPropertyType type);
        void Register(IComponent component);
        void Register(ITable table);

        IPType Type();
        IPTypeSpec Type(IPType ipType);
        IPTypeSpec Type(Guid id);

        IPTypeSet TypeSet(List<IPType> types);

        IPTypeSet TypeSet(List<Guid> types);

        IPTypeSet TypeSet();

        IPProperty Property();

        IPMethod Method();

        //IPropertyType PropertyType();
        ITable Table();

        void AddMD(Guid id, Guid parentId, object metadata);

        void Verify();

        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
        void AddOrUpdateSetting(IObjectSetting setting);
    }
}