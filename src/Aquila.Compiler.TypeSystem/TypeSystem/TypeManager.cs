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
    public class TypeManager
    {
        private readonly ITypeSystem _backend;
        private List<PType> _types;
        private List<PField> _fields;
        private List<PProperty> _properties;
        private List<PInvokable> _methods;
        private List<Component> _components;
        private List<ObjectSetting> _objectSettings;
        private List<MetadataRow> _metadatas;
        private List<BackendObject> _backendObjects;
        private List<PConstructor> _constructors;


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
            _types = new List<PType>();
            _properties = new List<PProperty>();
            _methods = new List<PInvokable>();
            _components = new List<Component>();
            _objectSettings = new List<ObjectSetting>();
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

        public PType Int => _intPType ??= new IntPType(this, _backend.GetSystemBindings().Int);
        public PType DateTime => _dateTimePType ??= new DateTimePType(this, _backend.GetSystemBindings().DateTime);

        public PType Binary =>
            _binaryPType ??= new BinaryPType(this, _backend.GetSystemBindings().Byte.MakeArrayType());

        public PType String => _stringPType ??= new StringPType(this, _backend.GetSystemBindings().String);

        public PType Boolean => _booleanPType ??= new BooleanPType(this, _backend.GetSystemBindings().Boolean);

        public PType Guid => _guidPType ??= new GuidPType(this, _backend.GetSystemBindings().Guid);

        public PType Numeric => _numericPType ??= new NumericPType(this, _backend.GetSystemBindings().Decimal);

        public PType Unknown => _unknownPType ??= new UnknownPType(this);

        public IReadOnlyList<PType> Types => _types;

        public IReadOnlyList<PMember> Members { get; }

        public IReadOnlyList<PProperty> Properties => _properties;

        public IReadOnlyList<PInvokable> Methods => _methods;

        public IReadOnlyList<PConstructor> Constructors => _constructors;

        public IReadOnlyList<PField> Fields => null;

        public IReadOnlyList<Component> Components => _components;

        public IReadOnlyList<ObjectSetting> Settings => _objectSettings;

        public IReadOnlyList<MetadataRow> Metadatas => _metadatas;

        public IReadOnlyList<BackendObject> BackendObjects => _backendObjects;

        internal void Register(PType ipType)
        {
            if (_types.Exists(x => x.Id == ipType.Id))
                throw new Exception($"Type id {ipType.Name}:{ipType.Id} already registered");

            _types.Add(ipType);
        }

        internal void Register(PField field)
        {
            if (_fields.Exists(x => x.Id == field.Id))
                throw new Exception($"Field id {field.Name}:{field.Id} already registered");

            _fields.Add(field);
        }


        internal void Register(PProperty p)
        {
            if (_properties.Exists(x => x.Id == p.Id && x.ParentId == p.ParentId))
                throw new Exception($"Property id {p.Name}:{p.Id} already registered");

            _properties.Add(p);
        }

        internal void Register(PInvokable method)
        {
            if (_methods.Exists(x => x.Id == method.Id && x.ParentId == method.ParentId))
                throw new Exception($"Method id {method.Name}:{method.Id} already registered");
            _methods.Add(method);
        }

        public void AddMD(Guid id, Guid parentId, object metadata)
        {
            _metadatas.Add(new MetadataRow {Id = id, ParentId = parentId, Metadata = metadata});
        }


        public Component Component()
        {
            var component = new Component(this);
            _components.Add(component);
            return component;
        }

        public PType ExportType(IType type)
        {
            return new PExportType(this, type);
        }

        public PTypeBuilder DefineType()
        {
            var tb = new PTypeBuilder(this);

            Register(tb);

            return tb;
        }

        public PTypeSpec DefineType(PType baseType)
        {
            var ts = new PTypeSpec(baseType.Id, this);

            Register(ts);

            return ts;
        }

        public PTypeSpec DefineType(Guid baseTypeId)
        {
            var ts = new PTypeSpec(baseTypeId, this);

            Register(ts);

            return ts;
        }

        public PTypeSet DefineTypeSet(List<PType> types)
        {
            var ts = new PTypeSet(types, this);

            Register(ts);

            return ts;
        }

        public PTypeSet DefineTypeSet(List<Guid> types)
        {
            var ts = new PTypeSet(types, this);

            Register(ts);

            return ts;
        }

        public PTypeSet DefineTypeSet()
        {
            var ts = new PTypeSet(this);

            Register(ts);

            return ts;
        }

        public void AddOrUpdateSetting(ObjectSetting setting)
        {
            _objectSettings.RemoveAll(x => x.ObjectId == setting.ObjectId);
            _objectSettings.Add(setting);
        }

        public void ConnectBackendObject(Guid id, object backendObject)
        {
            _backendObjects.Add(new BackendObject(backendObject, id));
        }

        public void LoadSettings(IEnumerable<ObjectSetting> settings)
        {
            _objectSettings = settings.ToList();
        }
    }
}