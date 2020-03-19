using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private List<Contracts.TypeSystem.IPType> _types;
        private List<IPProperty> _properties;
        private List<IPropertyType> _propertyTypes;
        private List<ITable> _tables;
        private List<IComponent> _components;
        private List<IObjectSetting> _objectSettings;
        private List<MetadataRow> _metadatas;

        private IntPType _intPType;
        private DateTimePType _dateTimePType;
        private BinaryPType _binaryPType;
        private StringPType _stringPType;
        private BooleanPType _booleanPType;
        private GuidPType _guidPType;
        private NumericPType _numericPType;


        public TypeManager()
        {
            _types = new List<Contracts.TypeSystem.IPType>();
            _properties = new List<IPProperty>();
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

            //register settings
            _objectSettings.Add(new ObjectSetting {ObjectId = _intPType.Id, SystemId = _intPType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _dateTimePType.Id, SystemId = _dateTimePType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _binaryPType.Id, SystemId = _binaryPType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _stringPType.Id, SystemId = _stringPType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _booleanPType.Id, SystemId = _booleanPType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _guidPType.Id, SystemId = _guidPType.SystemId});
            _objectSettings.Add(new ObjectSetting {ObjectId = _numericPType.Id, SystemId = _numericPType.SystemId});
        }

        public Contracts.TypeSystem.IPType Int => _intPType ??= new IntPType(this);
        public Contracts.TypeSystem.IPType DateTime => _dateTimePType ??= new DateTimePType(this);
        public Contracts.TypeSystem.IPType Binary => _binaryPType ??= new BinaryPType(this);
        public Contracts.TypeSystem.IPType String => _stringPType ??= new StringPType(this);
        public Contracts.TypeSystem.IPType Boolean => _booleanPType ??= new BooleanPType(this);
        public Contracts.TypeSystem.IPType Guid => _guidPType ??= new GuidPType(this);
        public Contracts.TypeSystem.IPType Numeric => _numericPType ??= new NumericPType(this);

        public IReadOnlyList<Contracts.TypeSystem.IPType> Types => _types;

        public IReadOnlyList<IPProperty> Properties => _properties;

        public IReadOnlyList<IPropertyType> PropertyTypes => _propertyTypes;

        public IReadOnlyList<ITable> Tables => _tables;

        public IReadOnlyList<IComponent> Components => _components;

        public IReadOnlyList<IObjectSetting> Settings => _objectSettings;

        public IReadOnlyList<IMetadataRow> Metadatas => _metadatas;

        public void Register(Contracts.TypeSystem.IPType ipType)
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

        public void Register(IComponent component)
        {
            _components.Add(component);
        }

        public void Register(ITable table)
        {
            _tables.Add(table);
        }

        public void AddMD(Guid id, Guid parentId, object metadata)
        {
            _metadatas.Add(new MetadataRow {Id = id, ParentId = parentId, Metadata = metadata});
        }

        public IComponent Component()
        {
            return new Component(this);
        }

        public Contracts.TypeSystem.IPType Type()
        {
            return new PType(this);
        }

        public IPTypeSpec Type(Contracts.TypeSystem.IPType ipType)
        {
            return new PTypeSpec(ipType, this);
        }

        public IPProperty Property()
        {
            return new PProperty(this);
        }

        public IPropertyType PropertyType()
        {
            return new PropertyType(this);
        }

        public ITable Table()
        {
            return new Table(this);
        }

        public void AddOrUpdateSetting(IObjectSetting setting)
        {
            _objectSettings.RemoveAll(x => x.ObjectId == setting.ObjectId);
            _objectSettings.Add(setting);
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