using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;
using Type = ZenPlatform.Configuration.TypeSystem.Type;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private List<IType> _types;
        private List<IProperty> _properties;
        private List<IPropertyType> _propertyTypes;
        private List<ITable> _tables;
        private List<IComponent> _components;
        private List<IObjectSetting> _objectSettings;
        private List<MetadataRow> _metadatas;

        private IntType _intType;
        private DateTimeType _dateTimeType;
        private BinaryType _binaryType;
        private StringType _stringType;
        private BooleanType _booleanType;
        private GuidType _guidType;
        private NumericType _numericType;


        public TypeManager()
        {
            _types = new List<IType>();
            _properties = new List<IProperty>();
            _propertyTypes = new List<IPropertyType>();
            _tables = new List<ITable>();
            _components = new List<IComponent>();
            _objectSettings = new List<IObjectSetting>();

            _types.Add(Int);
            _types.Add(DateTime);
            _types.Add(Binary);
            _types.Add(String);
            _types.Add(Boolean);
            _types.Add(Guid);
            _types.Add(Numeric);
        }

        public IType Int => _intType ??= new IntType(this);
        public IType DateTime => _dateTimeType ??= new DateTimeType(this);
        public IType Binary => _binaryType ??= new BinaryType(this);
        public IType String => _stringType ??= new StringType(this);
        public IType Boolean => _booleanType ??= new BooleanType(this);
        public IType Guid => _guidType ??= new GuidType(this);
        public IType Numeric => _numericType ??= new NumericType(this);

        public IReadOnlyList<IType> Types => _types;

        public IReadOnlyList<IProperty> Properties => _properties;

        public IReadOnlyList<IPropertyType> PropertyTypes => _propertyTypes;

        public IReadOnlyList<ITable> Tables => _tables;

        public IReadOnlyList<IComponent> Components => _components;

        public IReadOnlyList<IObjectSetting> Settings => _objectSettings;

        public IReadOnlyList<IMetadataRow> Metadatas => _metadatas;

        public void Register(IType type)
        {
            if (_types.Exists(x => x.Id == type.Id))
                throw new Exception($"Type id {type.Name}:{type.Id} already registered");

            _types.Add(type);
        }

        public void Register(IProperty p)
        {
            if (_properties.Exists(x => x.Id == p.Id))
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

        public void Register(MetadataRow md)
        {
            _metadatas.Add(md);
        }

        public IComponent Component()
        {
            return new Component(this);
        }

        public IType Type()
        {
            return new Type(this);
        }

        public ITypeSpec Type(IType type)
        {
            return new TypeSpec(type, this);
        }

        public IProperty Property()
        {
            return new Property(this);
        }

        public IPropertyType PropertyType()
        {
            return new PropertyType(this);
        }

        public ITable Table()
        {
            return new Table(this);
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