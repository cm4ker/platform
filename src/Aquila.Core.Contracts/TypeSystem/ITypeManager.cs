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
        IReadOnlyList<IPConstructor> Constructors { get; }

        IPTypeBuilder DefineType();

        IPTypeSpec DefineType(IPType baseType);

        IPTypeSpec DefineType(Guid baseTypeId);

        IPTypeSet DefineTypeSet(List<IPType> types);

        IPTypeSet DefineTypeSet(List<Guid> types);

        IPTypeSet DefineTypeSet();

        IPTypeBuilder NestedType(Guid parentId);

        void AddMD(Guid id, Guid parentId, object metadata);

        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
        void AddOrUpdateSetting(IObjectSetting setting);
    }
}