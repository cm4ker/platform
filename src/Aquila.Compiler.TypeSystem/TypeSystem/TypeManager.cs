using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Aqua.TypeSystem.StandartTypes;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private readonly ITypeSystem _backend;
        private List<IPType> _types;
        private List<IPProperty> _properties;
        private List<IPInvokable> _methods;
        private List<IPropertyType> _propertyTypes;
        private List<ITable> _tables;
        private List<IComponent> _components;
        private List<IObjectSetting> _objectSettings;
        private List<MetadataRow> _metadatas;
        private List<BackendObject> _backendObjects;

        private IntPType _intPType;
        private DateTimePType _dateTimePType;
        private BinaryPType _binaryPType;
        private StringPType _stringPType;
        private BooleanPType _booleanPType;
        private GuidPType _guidPType;
        private NumericPType _numericPType;
        private UnknownPType _unknownPType;

        public TypeManager(ITypeSystem backend)
        {
            _backend = backend;
            _types = new List<IPType>();
            _properties = new List<IPProperty>();
            _methods = new List<IPInvokable>();
            _propertyTypes = new List<IPropertyType>();
            _tables = new List<ITable>();
            _components = new List<IComponent>();
            _objectSettings = new List<IObjectSetting>();
            _metadatas = new List<MetadataRow>();

            _types.Add(Int);
            _types.Add(DateTime);
            _types.Add(Binary);
            _types.Add(String);
            _types.Add(Boolean);
            _types.Add(Guid);
            _types.Add(Numeric);
            _types.Add(Unknown);
        }

        public IPType Int => _intPType ??= new IntPType(this, _backend.GetSystemBindings().Int);
        public IPType DateTime => _dateTimePType ??= new DateTimePType(this, _backend.GetSystemBindings().DateTime);

        public IPType Binary =>
            _binaryPType ??= new BinaryPType(this, _backend.GetSystemBindings().Byte.MakeArrayType());

        public IPType String => _stringPType ??= new StringPType(this, _backend.GetSystemBindings().String);

        public IPType Boolean => _booleanPType ??= new BooleanPType(this, _backend.GetSystemBindings().Boolean);

        public IPType Guid => _guidPType ??= new GuidPType(this, _backend.GetSystemBindings().Guid);

        public IPType Numeric => _numericPType ??= new NumericPType(this, _backend.GetSystemBindings().Decimal);

        public IPType Unknown => _unknownPType ??= new UnknownPType(this);

        public IReadOnlyList<IPType> Types => _types;
        public IReadOnlyList<IPMember> Members { get; }

        public IReadOnlyList<IPProperty> Properties => _properties;

        public IReadOnlyList<IPInvokable> Methods => _methods;

        public IReadOnlyList<IPropertyType> PropertyTypes => _propertyTypes;

        public IReadOnlyList<ITable> Tables => _tables;

        public IReadOnlyList<IComponent> Components => _components;

        public IReadOnlyList<IObjectSetting> Settings => _objectSettings;

        public IReadOnlyList<IMetadataRow> Metadatas => _metadatas;

        public IReadOnlyList<IBackendObject> BackendObjects => _backendObjects;

        public void Register(IPType ipType)
        {
            if (_types.Exists(x => x.Id == ipType.Id))
                throw new Exception($"Type id {ipType.Name}:{ipType.Id} already registered");

            _types.Add(ipType);
        }

        public void Register(IPProperty p)
        {
            if (_properties.Exists(x => x.Id == p.Id && x.ParentId == p.ParentId))
                throw new Exception($"Property id {p.Name}:{p.Id} already registered");

            _properties.Add(p);
        }

        public void Register(IPropertyType type)
        {
            _propertyTypes.Add(type);
        }

        public void Register(IPInvokable method)
        {
            if (_methods.Exists(x => x.Id == method.Id && x.ParentId == method.ParentId))
                throw new Exception($"Method id {method.Name}:{method.Id} already registered");
            _methods.Add(method);
        }

        public void Register(IComponent component)
        {
            _components.Add(component);
        }

        public void Register(ITable table)
        {
            _tables.Add(table);
        }

        public ITable NestedType()
        {
            throw new NotImplementedException();
        }

        public void AddMD(Guid id, Guid parentId, object metadata)
        {
            _metadatas.Add(new MetadataRow {Id = id, ParentId = parentId, Metadata = metadata});
        }

        public IComponent Component()
        {
            return new Component(this);
        }

        public IPTypeBuilder Type()
        {
            return new PTypeBuilder(this);
        }

        public IPTypeSpec Type(IPType baseType)
        {
            return new PTypeSpec(baseType.Id, this);
        }

        public IPTypeSpec Type(Guid baseTypeId)
        {
            return new PTypeSpec(baseTypeId, this);
        }

        public IPTypeSet TypeSet(List<IPType> types)
        {
            return new PTypeSet(types, this);
        }

        public IPTypeSet TypeSet(List<Guid> types)
        {
            return new PTypeSet(types, this);
        }

        public IPTypeSet TypeSet()
        {
            return new PTypeSet(this);
        }

        public IPProperty Property(Guid id, Guid parentId)
        {
            return new PProperty(id, parentId, this);
        }

        public IPProperty Property(Guid parentId)
        {
            return new PProperty(parentId, this);
        }

        public IPMethod Method(Guid id, Guid parentId)
        {
            return new PMethod(id, parentId, this);
        }

        public IPMethod Method(Guid parentId)
        {
            return new PMethod(parentId, this);
        }

        public IPConstructor Constructor(Guid id, Guid parentId)
        {
            return new PConstructor(id, parentId, this);
        }

        public IPConstructor Constructor(Guid parentId)
        {
            return new PConstructor(parentId, this);
        }

        public IPType NestedType(Guid parentId)
        {
            return new NestedType(System.Guid.NewGuid(), parentId, this);
        }

        public void AddOrUpdateSetting(IObjectSetting setting)
        {
            _objectSettings.RemoveAll(x => x.ObjectId == setting.ObjectId);
            _objectSettings.Add(setting);
        }

        public void ConnectBackendObject(Guid id, object backendObject)
        {
            _backendObjects.Add(new BackendObject(backendObject, id));
        }

        public void LoadSettings(IEnumerable<IObjectSetting> settings)
        {
            _objectSettings = settings.ToList();
        }

        public void Verify()
        {
        }
    }
}