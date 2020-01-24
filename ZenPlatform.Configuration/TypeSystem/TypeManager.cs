using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private List<IType> _types;
        private List<IProperty> _properties;
        private List<IPropertyType> _propertyTypes;
        private List<ITable> _tables;
        private List<IComponent> _components;


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

            //Register base types
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

        public IReadOnlyList<ITable> Tables => _tables;

        public IReadOnlyList<IComponent> Components => _components;

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

        public void Regsiter(IComponent component)
        {
            _components.Add(component);
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

        public void Verify()
        {
        }
    }
}