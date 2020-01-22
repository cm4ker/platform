using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ConfCore
{
    class Program
    {
        static void Main(string[] args)
        {
            TypeSystem ts = new TypeSystem();

            ts.Type(new BinaryType(ts));
        }
    }

    /*
     CreateDataType -> Create MD -> Create derivative MD
 
     _type
     {
        Id,            
        ParentId,
        
        Name,
        
        IsRoot,
        IsDerivative,
        IsPrimitive,
        
        IsAbstract
        IsLink
     }
     
     _prop
     {
        Id,
        Name,
        OwnerTypeId     
     }
     
     _prop_types
     {
        PropId,
        TypeId,   
        
        Scale,
        Precision,
        Size                  
     }

    RealClass Real Class Real Class                  
             \    |    /
 Real Class -- METADATA -- Real Class
             /    |    \
    RealClass Real Class Real Class
    
    
           TypeSystem
               |
    MD -> PConstructor -> Some Classes
    
    Runtime    - Read Only
    Construct  - Read Write
    
    
    У нас есть 3 вида уровня
    
    1) Уровень метаданных 
    2) Уровень платформы - Может быть множество объектв
    3) Уровень сборки - Может быть множество классов
    
    
    */

    public class Type
    {
        private readonly TypeSystem _ts;

        internal Type(TypeSystem ts)
        {
            _ts = ts;
        }

        public virtual Guid Id { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual uint SystemId { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsLink { get; set; }

        public virtual bool IsObject { get; set; }

        public virtual bool IsManager { get; set; }

        public virtual bool IsDto { get; set; }

        public virtual bool IsPrimitive { get; set; }

        public virtual bool IsValue { get; set; }

        public virtual bool IsSizable { get; set; }

        public virtual bool IsScalePrecision { get; set; }

        //TODO: Use scope in main project
        public bool IsCodeAccess { get; set; }

        public virtual bool IsTypeSpec => false;

        public Metadata Metadata { get; set; }

        public object Bag { get; set; }

        public IEnumerable<Property> Properties => _ts.Properties.Where(x => x.ParentId == Id);

        public TypeSpec GetSpec()
        {
            return _ts.Type(this);
        }
    }

    public class TypeSpec : Type
    {
        private readonly Type _type;
        private string _name;


        internal TypeSpec(Type type, TypeSystem ts) : base(ts)
        {
            _type = type;
        }

        public override string Name => _name ??= "";

        public override Guid? ParentId => _type.Id;

        public int Scale { get; set; }

        public int Precision { get; set; }

        public int Size { get; set; }

        public bool IsNullable { get; set; }

        public override bool IsTypeSpec => true;

        private string CalcName()
        {
            var result = _type.Name;

            if (IsNullable)
                result += "?";

            if (_type.IsSizable)
            {
                
            }
        }
    }

    public class Property
    {
        internal Property(TypeSystem ts)
        {
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsReadOnly { get; set; }
    }

    public class PropertyType
    {
        public PropertyType(TypeSystem ts)
        {
        }

        public Guid PropertyId { get; set; }
        public Guid TypeId { get; set; }
    }

    public class Table
    {
        public Table(TypeSystem ts)
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }
    }

    public class Metadata
    {
    }

    public class TypeSystem
    {
        private List<Type> _types;
        private List<Property> _properties;
        private List<PropertyType> _propertyTypes;

        public TypeSystem()
        {
            _types = new List<Type>();
            _properties = new List<Property>();
            _propertyTypes = new List<PropertyType>();

            //Register base types
            _types.Add(new IntType(this));
            _types.Add(new DateTimeType(this));
            _types.Add(new BinaryType(this));
            _types.Add(new StringType(this));
            _types.Add(new BooleanType(this));
            _types.Add(new GuidType(this));
            _types.Add(new NumericType(this));
        }

        public IReadOnlyList<Type> Types => _types;

        public IReadOnlyList<Property> Properties => _properties;

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

        public void Verify()
        {
        }
    }
}