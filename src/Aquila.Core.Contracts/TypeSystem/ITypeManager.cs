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

        IPType Unknown { get; }

        IReadOnlyList<IPType> Types { get; }

        IReadOnlyList<IPMember> Members { get; }

        IReadOnlyList<IPProperty> Properties { get; }

        IReadOnlyList<IPInvokable> Methods { get; }

        IReadOnlyList<ITable> Tables { get; }
        IReadOnlyList<IComponent> Components { get; }
        IReadOnlyList<IObjectSetting> Settings { get; }

        IReadOnlyList<IMetadataRow> Metadatas { get; }
        IReadOnlyList<IBackendObject> BackendObjects { get; }

        void Register(IPType ipType);
        void Register(IPProperty p);
        void Register(IPInvokable method);
        void Register(IPropertyType type);
        void Register(IComponent component);
        void Register(ITable table);

        IPTypeBuilder Type();

        IPTypeSpec Type(IPType baseType);

        IPTypeSpec Type(Guid baseTypeId);

        IPTypeSet TypeSet(List<IPType> types);

        IPTypeSet TypeSet(List<Guid> types);

        IPTypeSet TypeSet();

        ITable NestedType();

        void AddMD(Guid id, Guid parentId, object metadata);

        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
        void AddOrUpdateSetting(IObjectSetting setting);
    }
}