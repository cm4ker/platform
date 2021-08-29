using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Metadata
{
    public class SMCache
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

        public void AddType(SMEntity metadata)
        {
            _types.Add(new SMType(metadata));
        }
    }

    public class SMEntity
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
            _md = md;
            _cache = cache;
        }

        private List<SMProperty> _props;
        private SMProperty _idProperty;

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

        public SMProperty FindProperty(string name) => GetPropertiesCore().FirstOrDefault(x => x.Name == name);

        public SMProperty IdProperty
        {
            get
            {
                CoreLazyPropertiesOrdered();
                return _idProperty;
            }
        }

        public void AddProperty(SMProperty prop)
        {
            if (_props.Exists(x => x.Name == prop.Name))
                throw new Exception($"Property with name {prop.Name} already declared");

            prop.UpdateParent(this);
            _props.Add(prop);
        }

        IEnumerable<SMProperty> GetPropertiesCore()
        {
            if (_props == null)
                CoreLazyPropertiesOrdered();

            return _props;
        }

        public IEnumerable<SMProperty> Properties => GetPropertiesCore();


        public string Name => _md.Name;

        public EntityMetadata Metadata => _md;


        /// <summary>
        /// Full name use in descriptor
        /// </summary>
        public string FullName => $"{Namespace}.{Name}";

        public string ReferenceName => $"{FullName}{LinkPostfix}";
    }

    public class SMProperty
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

        public SMEntity Parent { get; private set; }

        public bool IsIdProperty => Name == "Id";

        internal void UpdateParent(SMEntity entity)
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
    }

    public class SMType : IEquatable<SMType>
    {
        private readonly MetadataType _type;
        private readonly string _name;
        private readonly int _size;
        private readonly int _scale;
        private readonly int _precision;
        private readonly bool _isReference;
        private SMEntity _semanticMetadata;


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


        public SMType(MetadataType type)
        {
            _type = type;
            _name = _type.Name;

            _scale = _type.Scale;
            _precision = _type.Precision;
            _size = _type.Size;
        }

        public SMType(SMEntity semanticMetadata)
        {
            _semanticMetadata = semanticMetadata;

            //TODO: refactor this. We need divide reference from metadata 
            _name = semanticMetadata.ReferenceName;
            //Scale, precision, size is default at this case
        }

        public SMType(string name, int size = 0, int scale = 0, int precision = 0)
        {
            _name = name;
            _size = size;
            _scale = scale;
            _precision = precision;
        }

        public bool Equals(SMType? other)
        {
            return other != null
                   && Kind == other.Kind
                   && Size == other.Size
                   && Scale == other.Scale
                   && Precision == other.Precision;
        }

        public string Name => _name;


        public SMTypeKind Kind
        {
            get
            {
                if (_semanticMetadata == null)
                {
                    return Name switch
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
                else
                {
                    return SMTypeKind.Reference;
                }
            }
        }

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
    }
}