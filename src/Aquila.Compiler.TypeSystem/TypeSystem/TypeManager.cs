using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Aqua.TypeSystem.StandartTypes;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private readonly ITypeSystem _backend;
        private List<IPType> _types;
        private List<IPField> _fields;
        private List<IPProperty> _properties;
        private List<IPInvokable> _methods;
        private List<IPropertyType> _propertyTypes;
        private List<ITable> _tables;
        private List<IComponent> _components;
        private List<IObjectSetting> _objectSettings;
        private List<MetadataRow> _metadatas;
        private List<BackendObject> _backendObjects;
        private List<IPConstructor> _constructors;


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

        public IReadOnlyList<IPConstructor> Constructors => _constructors;

        public IReadOnlyList<IPField> Fields => null;

        public IReadOnlyList<ITable> Tables => _tables;

        public IReadOnlyList<IComponent> Components => _components;

        public IReadOnlyList<IObjectSetting> Settings => _objectSettings;

        public IReadOnlyList<IMetadataRow> Metadatas => _metadatas;

        public IReadOnlyList<IBackendObject> BackendObjects => _backendObjects;

        internal void Register(IPType ipType)
        {
            if (_types.Exists(x => x.Id == ipType.Id))
                throw new Exception($"Type id {ipType.Name}:{ipType.Id} already registered");

            _types.Add(ipType);
        }

        internal void Register(IPField field)
        {
            if (_fields.Exists(x => x.Id == field.Id))
                throw new Exception($"Field id {field.Name}:{field.Id} already registered");

            _fields.Add(field);
        }


        internal void Register(IPProperty p)
        {
            if (_properties.Exists(x => x.Id == p.Id && x.ParentId == p.ParentId))
                throw new Exception($"Property id {p.Name}:{p.Id} already registered");

            _properties.Add(p);
        }

        internal void Register(IPInvokable method)
        {
            if (_methods.Exists(x => x.Id == method.Id && x.ParentId == method.ParentId))
                throw new Exception($"Method id {method.Name}:{method.Id} already registered");
            _methods.Add(method);
        }

        public void AddMD(Guid id, Guid parentId, object metadata)
        {
            _metadatas.Add(new MetadataRow {Id = id, ParentId = parentId, Metadata = metadata});
        }


        public IComponent Component()
        {
            var component = new Component(this);
            _components.Add(component);
            return component;
        }

        public IPType ExportType(IType type)
        {
            return new PExportType(this, type);
        }

        public IPTypeBuilder DefineType()
        {
            var tb = new PTypeBuilder(this);

            Register(tb);

            return tb;
        }

        public IPTypeSpec DefineType(IPType baseType)
        {
            var ts = new PTypeSpec(baseType.Id, this);

            Register(ts);

            return ts;
        }

        public IPTypeSpec DefineType(Guid baseTypeId)
        {
            var ts = new PTypeSpec(baseTypeId, this);

            Register(ts);

            return ts;
        }

        public IPTypeSet DefineTypeSet(List<IPType> types)
        {
            var ts = new PTypeSet(types, this);

            Register(ts);

            return ts;
        }

        public IPTypeSet DefineTypeSet(List<Guid> types)
        {
            var ts = new PTypeSet(types, this);

            Register(ts);

            return ts;
        }

        public IPTypeSet DefineTypeSet()
        {
            var ts = new PTypeSet(this);

            Register(ts);

            return ts;
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
    }
}