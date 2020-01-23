using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ConfCore
{
    public class TypeSystem
    {
        private List<Type> _types;
        private List<Property> _properties;
        private List<PropertyType> _propertyTypes;
        private List<Table> _tables;
        private IntType _intType;
        private DateTimeType _dateTimeType;
        private BinaryType _binaryType;
        private StringType _stringType;
        private BooleanType _booleanType;
        private GuidType _guidType;
        private NumericType _numericType;

        public TypeSystem()
        {
            _types = new List<Type>();
            _properties = new List<Property>();
            _propertyTypes = new List<PropertyType>();

            //Register base types
            _types.Add(Int);
            _types.Add(DateTime);
            _types.Add(Binary);
            _types.Add(String);
            _types.Add(Boolean);
            _types.Add(Guid);
            _types.Add(Numeric);
        }

        public Type Int => _intType ??= new IntType(this);
        public Type DateTime => _dateTimeType ??= new DateTimeType(this);
        public Type Binary => _binaryType ??= new BinaryType(this);
        public Type String => _stringType ??= new StringType(this);
        public Type Boolean => _booleanType ??= new BooleanType(this);
        public Type Guid => _guidType ??= new GuidType(this);
        public Type Numeric => _numericType ??= new NumericType(this);

        public IReadOnlyList<Type> Types => _types;

        public IReadOnlyList<Property> Properties => _properties;

        public IReadOnlyList<Table> Tables => _tables;

        public void Register(Type type)
        {
            if (_types.Exists(x => x.Id == type.Id))
                throw new Exception($"Type id {type.Name}:{type.Id} already registered");

            _types.Add(type);
        }

        public void Register(Property p)
        {
            if (_properties.Exists(x => x.Id == p.Id))
                throw new Exception($"Property id {p.Name}:{p.Id} already registered");

            _properties.Add(p);
        }

        public void Register(PropertyType type)
        {
            _propertyTypes.Add(type);
        }

        public Type Type()
        {
            return new Type(this);
        }

        public TypeSpec Type(Type type)
        {
            return new TypeSpec(type, this);
        }

        public Property Property()
        {
            return new Property(this);
        }

        public PropertyType PropertyType()
        {
            return new PropertyType(this);
        }

        public Table Table()
        {
            return new Table(this);
        }

        public void Verify()
        {
        }
    }
}