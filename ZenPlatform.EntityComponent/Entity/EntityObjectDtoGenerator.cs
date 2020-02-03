using System;
using System.Data.Common;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Root = ZenPlatform.Language.Ast.Definitions.Root;

namespace ZenPlatform.EntityComponent.Entity
{
    public class EntityObjectDtoGenerator
    {
        private readonly IComponent _component;

        public EntityObjectDtoGenerator(IComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(IPType ipType, Root root)
        {
            var dtoClassName = ipType.GetDtoType().Name;

            var cls = new ComponentClass(CompilationMode.Shared, _component, ipType, null, dtoClassName,
                TypeBody.Empty) {Namespace = ipType.GetNamespace()};

            var cu = CompilationUnit.Empty;

            cu.AddEntity(cls);
            /*
             Табличные части именуются следующим принципом:
             TR_{ObjectName}_{TableName}
             
             public class Dto
             {
                public List<TableRow> Table1 { get; }      
             }
             
             public class ObjectTable1 : IEnumerable<TableRow>
             {
                List<TableRowObject> _list;
                
                public ObjectTable(List<TableRow> list)
                {
                    _list = list.Select(x=>new TableRowObject(x));
                }
                                
                public IEnumerable<TableRowObject> Table1 => _list;
                
                
                public Create()
                {
                    var o = new TableRow();
                    var wrap = new TableObjectRow(o);
                    
                    _dto.Add(o);
                    _table1.Add(wrap);
                    
                    retrun wrap;
                }         
             }
       
             
             
             public class Object
             {
                private ObjectTable1 _table1;
                private Dto _dto;
                
                public Object(Dto dto)
                {
                    _dto = dto;
                    _table1 = new ObjectTable(dto.Table1);        
                }
                
               public ObjectTable1 Table1 => _table1;                
                
                public TableRowObject CreateRowTable1()
                {
                   
                }
                
                public RemoveTable1()
                {
                    ...
                }
             }
             
             public class TableRowObject
             {
                Create()
                Remove()
                ...
             }
             
             public class TableRowLink
             {
                
             }
             
             public class TableRow
             {
                
             }
             
             */
            foreach (var table in ipType.Tables)
            {
                var tableName = $"TR{ipType.Name}_{table.Name}";

                var dtoTableCls = new ComponentClass(CompilationMode.Shared, _component, ipType, null,
                    tableName, TypeBody.Empty)
                {
                    Namespace = ipType.GetNamespace(),
                    Bag = table
                };

                cu.AddEntity(dtoTableCls);
            }

            root.Add(cu);
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
                if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Server))
                {
                    if (cc.Bag is ITable tab)
                    {
                        //if this is table row class
                        EmitTable(cc, builder, tab);
                    }
                    else
                    {
                        EmitBody(cc, builder, dbType);
                        EmitVersionField(builder);
                        EmitMappingSupport(cc, builder);
                    }
                }
                else if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Client))
                {
                    EmitBody(cc, builder, dbType);
                }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
        }

        private void EmitTable(ComponentClass cc, ITypeBuilder builder, ITable table)
        {
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var tm = table.TypeManager;

            var ownerType = tm.FindType(table.ParentId);
            var @namespace = ownerType.GetNamespace();

            var dtoType = ts.FindType($"{@namespace}.{table}");

            foreach (var prop in table.Properties)
            {
                EmitProperty(builder, prop, sb);
            }
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = cc.Type;

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                EmitProperty(builder, prop, sb);
            }

            // foreach (var table in set.Tables)
            // {
            //     var tableDto = ts.GetType(table.GetTableDtoName());
            //
            //     var result = builder.DefineProperty(tableDto, table.Name, true, true, false);
            // }
        }


        private void EmitProperty(ITypeBuilder builder, IPProperty prop, SystemTypeBindings sb)
        {
            bool propertyGenerated = false;
            if (prop.IsSelfLink) return;


            if (string.IsNullOrEmpty(prop.GetSettings().DatabaseName))
            {
                throw new Exception(
                    $"Prop: {prop.Name} ObjectType: {"Empty"}. Database column is empty!");
            }

            if (prop.Types.Count() > 1)
            {
                var clsSchema = prop.GetObjSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);

                var dbSchema = prop.GetDbSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);


                var propBuilder = builder.DefinePropertyWithBackingField(clsSchema.PlatformIpType.ConvertType(sb),
                    clsSchema.FullName, false);

                var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                propBuilder.SetAttribute(attr);
                attr.SetParameters(dbSchema.FullName);
            }


            foreach (var ctype in prop.Types)
            {
                if (ctype.IsPrimitive)
                {
                    var dbColName = prop
                        .GetDbSchema()
                        .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                        ? ColumnSchemaType.Value
                                        : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    var propName = prop
                        .GetObjSchema()
                        .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                        ? ColumnSchemaType.Value
                                        : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    IType propType = ctype.ConvertType(sb);

                    var propBuilder = builder.DefinePropertyWithBackingField(propType, propName, false);

                    var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                    propBuilder.SetAttribute(attr);
                    attr.SetParameters(dbColName);

                    if (!prop.IsUnique)
                    {
                        var initAttr = builder.CreateAttribute<NeedInitAttribute>();
                        propBuilder.SetAttribute(initAttr);
                    }
                }
                else if (ctype.IsLink)
                {
                    if (!propertyGenerated)
                    {
                        propertyGenerated = true;

                        var dbColName = prop
                            .GetDbSchema()
                            .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                            ? ColumnSchemaType.Ref
                                            : ColumnSchemaType.NoSpecial)).FullName;

                        var propName = prop
                            .GetObjSchema()
                            .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                            ? ColumnSchemaType.Ref
                                            : ColumnSchemaType.NoSpecial)).FullName;

                        var propBuilder = builder.DefinePropertyWithBackingField(sb.Guid, propName, false);

                        var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                        propBuilder.SetAttribute(attr);
                        attr.SetParameters(dbColName);

                        if (!prop.IsUnique)
                        {
                            var initAttr = builder.CreateAttribute<NeedInitAttribute>();
                            propBuilder.SetAttribute(initAttr);
                        }
                    }
                }
            }
        }

        private void EmitMappingSupport(ComponentClass cls, ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();

            tb.AddInterfaceImplementation(_ts.FindType<ICanMap>());

            var readerMethod = tb.DefineMethod(nameof(ICanMap.Map), true, false, true);
            var rg = readerMethod.Generator;

            var readerType = _ts.FindType<DbDataReader>();

            var readerParam =
                readerMethod.DefineParameter("reader", readerType, false, false);

            foreach (var property in tb.Properties)
            {
                var mt = property.FindCustomAttribute<MapToAttribute>();
                if (mt is null) continue;

                rg
                    .LdArg_0()
                    .LdArg(readerParam.ArgIndex)
                    .LdStr(mt.Parameters[0].ToString())
                    .EmitCall(readerType.FindMethod("get_Item", _bindings.String))
                    .Unbox_Any(property.PropertyType)
                    .EmitCall(property.Setter);
            }

            rg.Ret();
        }

        private void EmitVersionField(ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _b = _ts.GetSystemBindings();
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version", false);
        }

        private void EmitDefaultValues(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = cc.Type;

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            foreach (var prop in set.Properties)
            {
                if (prop.IsSelfLink) continue;
            }
        }
    }


    /*
    private Guid <Id>k__BackingField;

	private string <Name>k__BackingField;

	private byte[] <Version>k__BackingField;

	[MapTo("Id")]
	public Guid Id
	{
		get
		{
			return <Id>k__BackingField;
		}
		set
		{
			<Id>k__BackingField = value;
		}
	}

	[MapTo("Name")]
	[NeedInit]
	public string Name
	{
		get
		{
			return <Name>k__BackingField;
		}
		set
		{
			<Name>k__BackingField = value;
		}
	}

	public byte[] Version
	{
		get
		{
			return <Version>k__BackingField;
		}
		set
		{
			<Version>k__BackingField = value;
		}
	}

!!! public List<TB__Department_Tb1> Tb1{ get; set; }

    

	public virtual void Map(DbDataReader reader)
	{
		Id = (Guid)reader["Id"];
		Name = (string)reader["Name"];
	}
     */
}