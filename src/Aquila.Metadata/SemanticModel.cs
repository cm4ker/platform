using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Aquila.Metadata
{
    public sealed class SMCache
    {
        private List<SMType> _types;

        public SMCache()
        {
            _types = new List<SMType>();
        }

        public SMType ResolveType(string name)
        {
            var result = _types.FirstOrDefault(x => x.Name == name);

            if (result == null)
                return new SMType(name);

            return result;
        }

        public void AddType(SMEntity metadata, string name, SMTypeKind typeKind)
        {
            _types.Add(new SMType(typeKind, metadata, name));
        }
    }

    public abstract class SMEntityOrTable : ISMPropertyHolder
    {
        public abstract string Name { get; } 
        
        public abstract string FullName { get; }

        public abstract IEnumerable<SMProperty> Properties { get; }

    }


    public sealed class SMEntity : SMEntityOrTable, IEquatable<SMEntity>
    {
        private readonly EntityMetadata _md;
        private readonly SMCache _cache;
        private const string Namespace = "Entity";
        private const string ObjectPostfix = "Object";
        private const string DtoPostfix = "Dto";
        private const string ManagerPostfix = "Manager";
        private const string LinkPostfix = "Link";

        private EntityProperty idProp = new EntityProperty
        {
            Name = "Id", Types = { new MetadataType { Name = SMType.Guid } }
        };

        internal SMEntity(EntityMetadata md, SMCache cache)
        {
            _md = md ?? throw new NullReferenceException("Unknown metadata");
            _cache = cache;
        }

        private List<SMProperty> _props;
        private SMProperty _idProperty;
        private List<SMTable> _tables;

        private void CoreLazyPropertiesOrdered()
        {
            _props = new List<SMProperty>();

            var id = new SMProperty(idProp, _cache);
            _idProperty = id;
            AddProperty(id);

            foreach (var p in _md.Properties.OrderBy(x => x.Name))
            {
                var prop = new SMProperty(p, _cache);
                AddProperty(prop);
            }
        }

        private void CoreLazyTablesOrdered()
        {
            _tables = new List<SMTable>();

            foreach (var t in _md.Tables.OrderBy(x => x.Name))
            {
                var table = new SMTable(t, _cache);
                AddTable(table);
            }
        }

        public SMProperty FindProperty(string name) => GetPropertiesCore().FirstOrDefault(x => x.Name == name);

        public SMProperty IdProperty
        {
            get
            {
                CoreLazyPropertiesOrdered();
                return _idProperty;
            }
        }

        private void AddProperty(SMProperty prop)
        {
            if (_props.Exists(x => x.Name == prop.Name))
                throw new Exception($"Property with name {prop.Name} already declared");

            prop.UpdateParent(this);
            _props.Add(prop);
        }

        private void AddTable(SMTable table)
        {
            if (_tables.Exists(x => x.Name == table.Name))
                throw new Exception($"Table with name {table.Name} already declared");

            table.UpdateParent(this);
            _tables.Add(table);
        }

        IEnumerable<SMProperty> GetPropertiesCore()
        {
            if (_props == null)
                CoreLazyPropertiesOrdered();

            return _props;
        }

        IEnumerable<SMTable> GetTablesCore()
        {
            if (_tables == null)
                CoreLazyTablesOrdered();

            return _tables;
        }

        public override IEnumerable<SMProperty> Properties => GetPropertiesCore();
        public IEnumerable<SMTable> Tables => GetTablesCore();


        public override string Name => _md.Name;

        public EntityMetadata Metadata => _md;


        /// <summary>
        /// Full name use in descriptor
        /// </summary>
        public override string FullName => $"{Namespace}.{Name}";

        public string ReferenceName => $"{FullName}{LinkPostfix}";

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;
                if (Properties.Any(x => !x.IsValid)) return false;

                return true;
            }
        }


        //TODO: make normal equality members. Now it uses only name

        public bool Equals(SMEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_md.Name, other._md.Name);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is SMEntity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_md.Name);
        }

        public static bool operator ==(SMEntity left, SMEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SMEntity left, SMEntity right)
        {
            return !Equals(left, right);
        }
    }

    public interface ISMPropertyHolder
    {
        string FullName { get; }
    }

    public sealed class SMProperty
    {
        private readonly EntityProperty _mdProp;
        private readonly SMCache _cache;
        private IEnumerable<SMType> _types;

        internal SMProperty(EntityProperty mdProp, SMCache cache)
        {
            _mdProp = mdProp;
            _cache = cache;
        }

        public string Name => _mdProp.Name;

        public string FullName => $"{Parent.FullName}.{Name}";

        public ISMPropertyHolder Parent { get; private set; }

        public bool IsIdProperty => Name == "Id";
        public bool IsParentProperty => Name == "Parent";

        internal void UpdateParent(ISMPropertyHolder entity)
        {
            Parent = entity;
        }

        void CoreUpdateTypes()
        {
            var resultTypes = new List<SMType>();
            foreach (var mdType in _mdProp.Types)
            {
                var semanticType = new SMType(mdType);

                if (semanticType.Kind == SMTypeKind.Unknown)
                {
                    //try to get metadata reference from the cache
                    var resolved = _cache.ResolveType(semanticType.Name);

                    if (resolved != null)
                        resultTypes.Add(resolved);
                }
                else
                {
                    resultTypes.Add(semanticType);
                }
            }

            _types = resultTypes;
        }

        public IEnumerable<SMType> Types
        {
            get
            {
                if (_types == null)
                    CoreUpdateTypes();
                return _types;
            }
        }

        public bool IsValid
        {
            get { return Types.All(x => !x.IsUnknown); }
        }
    }

    public sealed class SMTable : SMEntityOrTable
    {
        private readonly EntityTable _md;
        private readonly SMCache _cache;
        private List<SMProperty> _props;
        private SMProperty _parentProperty;

        private EntityProperty _parentProp = new EntityProperty
        {
            Name = "Parent", Types = { new MetadataType { Name = SMType.Guid } }
        };


        public SMTable(EntityTable md, SMCache cache)
        {
            _md = md;
            _cache = cache;
        }

        public override string Name => _md.Name;
        public override string FullName => $"{Parent.FullName}.{Name}";

        public SMEntity Parent { get; private set; }

        public SMProperty ParentProperty
        {
            get
            {
                CoreLazyPropertiesOrdered();
                return _parentProperty;
            }
        }
        
        internal void UpdateParent(SMEntity entity)
        {
            Parent = entity;
        }


        private void AddProperty(SMProperty prop)
        {
            if (_props.Exists(x => x.Name == prop.Name))
                throw new Exception($"Property with name {prop.Name} already declared");

            prop.UpdateParent(this);
            _props.Add(prop);
        }

        private void CoreLazyPropertiesOrdered()
        {
            _props = new List<SMProperty>();

            var id = new SMProperty(_parentProp, _cache);
            _parentProperty = id;
            AddProperty(id);

            foreach (var p in _md.Properties.OrderBy(x => x.Name))
            {
                var prop = new SMProperty(p, _cache);
                AddProperty(prop);
            }
        }

        IEnumerable<SMProperty> GetPropertiesCore()
        {
            if (_props == null)
                CoreLazyPropertiesOrdered();

            return _props;
        }

        public override IEnumerable<SMProperty> Properties => GetPropertiesCore();
    }

    public enum SMTypeKind
    {
        //Unknown type must be here because primitive type calculates
        //Like this Kind > Unknown And Kind < Reference
        Unknown = 0,

        String = 0x1,
        Int = 0x2,
        Long = 0x3,
        Bool = 0x4,
        Double = 0x5,
        Decimal = 0x6,
        DateTime = 0x7,
        Numeric = 0x8,
        Binary = 0x9,
        Guid = 0x10,

        //Important reference kind must be last below contains primitive types
        Reference = 0x100,

        //Object
        Object = 0x10000
    }

    public sealed class SMType : IEquatable<SMType>
    {
        private readonly MetadataType _type;
        private readonly string _name;
        private readonly int _size;
        private readonly int _scale;
        private readonly int _precision;
        private SMEntity _semanticMetadata;
        private SMTypeKind _typeKind;


        #region WellKnownTypes

        public const string String = "string";
        public const string Int = "int";
        public const string Boolean = "bool";
        public const string DateTime = "datetime";
        public const string Decimal = "decimal";
        public const string Numeric = "numeric";
        public const string Guid = "guid";
        public const string Unknown = "unknown";

        #endregion


        public SMType(MetadataType type) : this(type.Name, type.Size, type.Scale, type.Precision)
        {
            _type = type;
        }

        public SMType(SMTypeKind typeKind, SMEntity semanticMetadata, string name)
        {
            _semanticMetadata = semanticMetadata;
            _typeKind = typeKind;
            _name = name;
        }

        // public SMType(SMEntity semanticMetadata) : this(SMTypeKind.Reference, semanticMetadata,
        //     semanticMetadata.ReferenceName)
        // {
        // }

        public SMType(string name, int size = 0, int scale = 0, int precision = 0)
        {
            _name = name;
            _size = size;
            _scale = scale;
            _precision = precision;

            _typeKind = _name switch
            {
                String => SMTypeKind.String,
                Int => SMTypeKind.Int,
                Boolean => SMTypeKind.Bool,
                Decimal => SMTypeKind.Decimal,
                DateTime => SMTypeKind.DateTime,
                Numeric => SMTypeKind.Numeric,
                Guid => SMTypeKind.Guid,
                _ => SMTypeKind.Unknown
            };
        }


        public string Name => _name;


        public SMTypeKind Kind => _typeKind;

        public int Size => _size;
        public int Scale => _scale;
        public int Precision => _precision;

        public bool IsPrimitive => Kind > SMTypeKind.Unknown && Kind < SMTypeKind.Reference;
        public bool IsReference => Kind == SMTypeKind.Reference;
        public bool IsUnknown => Kind == SMTypeKind.Unknown;

        internal void UpdateSemanticMetadata(SMEntity semanticMetadata)
        {
            _semanticMetadata = semanticMetadata;
        }

        public static bool operator ==(SMType type1, SMType type2)
        {
            return type1?.Equals(type2) ?? type2 is null;
        }

        public static bool operator !=(SMType type1, SMType type2)
        {
            return !(type1 == type2);
        }

        public bool IsAssignableFrom(SMType t2)
        {
            return this._name == t2._name;
        }

        public SMEntity GetSemantic()
        {
            return _semanticMetadata;
        }

        public bool Equals(SMType? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _name == other._name && _size == other._size && _scale == other._scale &&
                   _precision == other._precision;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is SMType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_name, _size, _scale, _precision);
        }
    }

    public sealed class SMSecSubject
    {
        private readonly SecPolicySubjectMetadata _md;
        private readonly SMCache _cache;

        public SMSecSubject(SecPolicySubjectMetadata md, SMCache cache)
        {
            _md = md;
            _cache = cache;
        }

        public SecPermission Permission => _md.Permission;

        public SMEntity Subject => _cache.ResolveType(_md.Name).GetSemantic();
    }

    public sealed class SMSecPolicy
    {
        private readonly SecPolicyMetadata _md;
        private readonly SMCache _cache;

        public SMSecPolicy(SecPolicyMetadata md, SMCache cache)
        {
            _md = md;
            _cache = cache;
        }

        public string Name => _md.Name;


        public IEnumerable<SMSecSubject> Subjects
        {
            get { return _md.Subjects.Select(x => new SMSecSubject(x, _cache)); }
        }

        public IEnumerable<SMSecPolicyCriterion> Criteria =>
            _md.Criteria.Select(x => new SMSecPolicyCriterion(x, _cache));
    }

    public sealed class SMSecPolicyCriterion
    {
        private readonly SecPolicyCriterionMetadata _md;
        private readonly SMCache _cache;

        public SMSecPolicyCriterion(SecPolicyCriterionMetadata md, SMCache cache)
        {
            _md = md;
            _cache = cache;
        }

        public string Query => _md.Query;

        public SecPermission Permission => _md.Permission;

        public SMEntity Subject => _cache.ResolveType(_md.Subject).GetSemantic();
    }
}