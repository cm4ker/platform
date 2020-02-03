using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Bson;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;
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

        public void GenerateAstTree(ZenPlatform.Configuration.Contracts.TypeSystem.IType type, Root root)
        {
            var dtoClassName = type.GetDtoType().Name;

            var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, dtoClassName,
                TypeBody.Empty) {Namespace = type.GetNamespace()};

            cls.Bag = ObjectType.Dto;

            var cu = CompilationUnit.Empty;

            cu.AddEntity(cls);

            /*
             Табличные части именуются следующим принципом:
             TB_{ObjectName}_{TableName}
             */
            foreach (var table in type.Tables)
            {
                var dtoTableCls = new ComponentClass(CompilationMode.Shared, _component, type, null,
                    null, TypeBody.Empty)
                {
                    Namespace = type.GetNamespace(),
                    Bag = table
                };

                cu.AddEntity(dtoTableCls);
            }

            root.Add(cu);
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
                if ((ObjectType) cc.Bag == ObjectType.Dto)
                {
                    if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Server))
                    {
                        EmitBody(cc, builder, dbType);
                        EmitVersionField(builder);
                        EmitMappingSupport(cc, builder);
                    }
                    else if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Client))
                    {
                        EmitBody(cc, builder, dbType);
                    }
                }
                else if (cc.Bag is ITable tbl)
                {
                    EmitTable(cc, builder, tbl);
                }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
        }

        private void EmitTable(ComponentClass cc, ITypeBuilder builder, ITable table)
        {
            // var ts = builder.Assembly.TypeSystem;
            // //var set = table.;
            //
            // var dtoClassName = set.GetDtoName();
            // var @namespace = set.GetNamespace();
            //
            // var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");
            //
            // foreach (var prop in table.GetProperties())
            // {
            // }
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = cc.Type;

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;
                if (prop.IsSelfLink) continue;


                if (string.IsNullOrEmpty(prop.GetSettings().DatabaseName))
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {"Empty"} Name: {set.Name}. Database column is empty!");
                }

                if (prop.Types.Count() > 1)
                {
                    var clsSchema = prop.GetObjSchema()
                        .First(x => x.SchemaType == ColumnSchemaType.Type);

                    var dbSchema = prop.GetDbSchema()
                        .First(x => x.SchemaType == ColumnSchemaType.Type);


                    var propBuilder = builder.DefinePropertyWithBackingField(clsSchema.PlatformType.ConvertType(sb),
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
                                            : ColumnSchemaType.NoSpecial) && x.PlatformType == ctype).FullName;

                        var propName = prop
                            .GetObjSchema()
                            .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                            ? ColumnSchemaType.Value
                                            : ColumnSchemaType.NoSpecial) && x.PlatformType == ctype).FullName;

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

            // foreach (var table in set.Tables)
            // {
            //     var tableDto = ts.GetType(table.GetTableDtoName());
            //
            //     var result = builder.DefineProperty(tableDto, table.Name, true, true, false);
            // }
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